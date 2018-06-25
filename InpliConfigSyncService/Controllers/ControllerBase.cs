namespace InpliConfigSyncService.Controllers
{
    using Couchbase;
    using Couchbase.Authentication;
    using Couchbase.Configuration.Client;
    using Couchbase.Core.Serialization;
    using InpliDataModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public class ControllerBase<T> : Controller
    {
        protected IConfiguration Configuration { get; set; }

        protected Cluster DbCluster { get; set; }

        protected string BucketName { get; }

        public string TypeString { get; }

        public string DocumentPrefix { get; }

        public ControllerBase(IConfiguration configuration)
        {
            Configuration = configuration;

            var couchbaseBootstrapServers =
                configuration
                    .GetSection("Couchbase:BootstrapServers")
                    .AsEnumerable()
                    .Where(x => x.Value != null)
                    .Select(x =>
                        new Uri(x.Value)
                    )
                    .ToList();

            BucketName = configuration["Infrastructure:Bucket:Name"];
            var couchbaseUsername = configuration["Infrastructure:Bucket:Username"];
            var couchbasePassword = configuration["Infrastructure:Bucket:Password"];
            DocumentPrefix = configuration["Infrastructure:DocumentPrefix"];

            DbCluster = new Cluster(new ClientConfiguration
            {
                Servers = couchbaseBootstrapServers,
                Serializer = () =>
                {
                    JsonSerializerSettings serializerSettings =
                        new JsonSerializerSettings()
                        {
                            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                            DateTimeZoneHandling = DateTimeZoneHandling.Utc
                        };

                    serializerSettings.Converters.Add(new IPAddressConverter());
                    serializerSettings.Converters.Add(new IPEndPointConverter());

                    return new DefaultSerializer(serializerSettings, serializerSettings);
                }
            });

            var authenticator = new PasswordAuthenticator(couchbaseUsername, couchbasePassword);
            DbCluster.Authenticate(authenticator);

            var instanceOfT = (T)Activator.CreateInstance(typeof(T));
            TypeString = (string)(instanceOfT.GetType().GetProperty("Type").GetValue(instanceOfT, null));
        }

        protected async Task<List<T>> Get()
        {

            using (var bucket = DbCluster.OpenBucket(BucketName))
            {
                var queryResult = await DbCluster.QueryAsync<T>("SELECT `" + BucketName + "`.* FROM `" + BucketName + "` WHERE type=\"" + TypeString + "\"");
                if (queryResult.Success)
                    return queryResult.Rows;

                return new List<T>();
            }
        }

        protected async Task Post(T item)
        {
            var model = item as BaseModel;
            var classTypeString = typeof(T).Name;
            var idString = DocumentPrefix + classTypeString + "::" + model.Id.ToString();

            using (var bucket = DbCluster.OpenBucket(BucketName))
            {
                var insert = await bucket.InsertAsync(idString, model);
                if (insert.Success)
                {
                    Console.WriteLine("Added " + idString);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(insert.Status.ToString());
                }
            }
        }

        protected async Task Delete(Guid id)
        {
            var classTypeString = typeof(T).Name;
            var idString = DocumentPrefix + classTypeString + "::" + id.ToString();

            using (var bucket = DbCluster.OpenBucket(BucketName))
            {
                var insert = await bucket.RemoveAsync(idString);
                if (insert.Success)
                {
                    Console.WriteLine("Removed " + idString);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(insert.Status.ToString());
                }
            }
        }

        protected async Task Put(int id, [FromBody]T item)
        {
            var model = item as BaseModel;
            var classTypeString = typeof(T).Name;
            var idString = DocumentPrefix + classTypeString + "::" + model.Id.ToString();

            using (var bucket = DbCluster.OpenBucket(BucketName))
            {
                var insert = await bucket.UpsertAsync(idString, item);
                if (insert.Success)
                {
                    Console.WriteLine("Updated " + idString);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(insert.Status.ToString());
                }
            }
        }
    }
}
