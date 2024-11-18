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

        // I shared some code with ChatGpt and prompted it with
        // "for some reason, going to a different page and then
        // coming back will not prevent me from adding the same
        // boss into the list" and it gave me these methods to 
        // use when calling defeatedBosses.Contains(defeatedBoss)
        // on line 130 in Main Page

        // Override Equals to compare based on unique properties
        public override bool Equals(object obj)
        {
            if (obj is Boss otherBoss)
            {
                return Id == otherBoss.Id; // Compare based on Id
            }
            return false;
        }

        // Override GetHashCode to match Equals logic
        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }
}
