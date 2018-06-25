namespace InpliConfigSyncService.Services
{
    using SnmpSharpNet;
    using SnmpSharpNet.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    public class SnmpTools
    {
        public static Task SnmpSetAsync(IPAddress destination, string community, string oid, int value)
        {
            return SnmpSetAsync(destination, community, oid, (AsnType)(new Integer32(value)));
        }

        public static Task SnmpSetAsync(IPAddress destination, string community, string oid, string value)
        {
            return SnmpSetAsync(destination, community, oid, new OctetString(value));
        }

        public static Task SnmpSetAsync(IPAddress destination, string community, string oid, IPAddress value)
        {
            return SnmpSetAsync(destination, community, oid, new IpAddress(value));
        }

        public static Task SnmpSetAsync(IPAddress destination, string community, string oid, AsnType value)
        {
            return SnmpSetAsync(destination, community, new Oid(oid), value);
        }

        /// <summary>
        /// Perform an SNMP Set.
        /// </summary>
        /// <param name="destination">The destination host.</param>
        /// <param name="oid">The OID to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>Nothing.</returns>
        public static async Task SnmpSetAsync(IPAddress destination, string community, Oid oid, AsnType value)
        {
            UdpTarget target = new UdpTarget(destination);

            // Create a SET PDU
            Pdu pdu = new Pdu(EPduType.Set);

            pdu.VbList.Add(oid, value);

            // Set Agent security parameters
            AgentParameters aparam = new AgentParameters(ESnmpVersion.Ver2, new OctetString(community));

            // Response packet
            SnmpV2Packet response;
            try
            {
                // Send request and wait for response
                response = await target.RequestAsync(pdu, aparam) as SnmpV2Packet;
            }
            catch (Exception ex)
            {
                // If exception happens, it will be returned here
                Console.WriteLine(string.Format("Request failed with exception: {0}", ex.Message));
                target.Close();
                return;
            }

            // Make sure we received a response
            if (response == null)
            {
                Console.WriteLine("Error in sending SNMP request.");
            }
            else
            {
                // Check if we received an SNMP error from the agent
                if (response.Pdu.ErrorStatus != 0)
                {
                    Console.WriteLine(string.Format("SNMP agent returned ErrorStatus {0} on index {1}", response.Pdu.ErrorStatus, response.Pdu.ErrorIndex));
                }
                else
                {
                    // Everything is ok. Agent will return the new value for the OID we changed
                    Console.WriteLine(string.Format("Agent response {0}: {1}", response.Pdu[0].Oid.ToString(), response.Pdu[0].Value.ToString()));
                }
            }
        }
    }
}
