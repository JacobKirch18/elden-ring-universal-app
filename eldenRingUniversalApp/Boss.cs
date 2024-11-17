using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace eldenRingUniversalApp
{
    public class BossWrapper
    {
        public List<Boss> Data { get; set; }
    }
    public class Boss : INotifyPropertyChanged
    {
        private string id;
        public string Id
        {
            get => id;
            set
            {
                id = value;
                NotifyPropertyChanged();
            }
        }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }

        private string image;
        public string Image
        {
            get => image;
            set
            {
                image = value;
                NotifyPropertyChanged();
            }
        }

        private string region;

        public string Region
        {
            get => region;
            set
            {
                region = value;
                NotifyPropertyChanged();
            }
        }

        private string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                NotifyPropertyChanged();
            }
        }

        private string location;
        public string Location
        {
            get => location;
            set
            {
                location = value;
                NotifyPropertyChanged();
            }
        }

        private string[] drops;
        public string[] Drops
        {
            get => drops;
            set
            {
                drops = value;
                NotifyPropertyChanged();
            }
        }

        private string healthPoints;
        public string HealthPoints
        {
            get => healthPoints;
            set
            {
                healthPoints = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
