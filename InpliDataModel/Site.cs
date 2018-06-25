namespace InpliDataModel
{
    using System;
    using System.ComponentModel;

    public class Site : BaseModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Guid _id;
        private Guid _tenantId;
        private string _name;
        private string _location;
        private string _notes;

        public override string Type => "Infrastructure::Site";

        public override Guid Id
        {
            get => _id;
            set { PropertyChanged.ChangeAndNotify(ref _id, value, () => Id); }
        }

        public Guid TenantId
        {
            get => _tenantId;
            set { PropertyChanged.ChangeAndNotify(ref _tenantId, value, () => TenantId); }
        }

        public string Name
        {
            get => _name;
            set { PropertyChanged.ChangeAndNotify(ref _name, value, () => Name); }
        }

        public string Location
        {
            get => _location;
            set { PropertyChanged.ChangeAndNotify(ref _location, value, () => Location); }
        }

        public string Notes
        {
            get => _notes;
            set { PropertyChanged.ChangeAndNotify(ref _notes, value, () => Notes); }
        }
    }
}
