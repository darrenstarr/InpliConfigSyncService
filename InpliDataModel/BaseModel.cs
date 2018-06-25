namespace InpliDataModel
{
    using System;

    public abstract class BaseModel
    {
        public abstract string Type { get; }
        public abstract Guid Id { get; set; }
    }
}
