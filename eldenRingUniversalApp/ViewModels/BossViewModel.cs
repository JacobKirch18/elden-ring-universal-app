using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace eldenRingUniversalApp
{
    public class BossViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Boss boss;

        public BossViewModel()
        {
            this.boss = new Boss();
        }

        public string Id 
        {
            get {  return boss.Id; }
            set
            {
                boss.Id = value;
                NotifyPropertyChanged();
            }
        }

        public string Name 
        {
            get { return boss.Name; }
            set
            {
                boss.Name = value;
                NotifyPropertyChanged();
            }
        }

        public string Nickname
        {
            get { return boss.Nickname; }
            set
            {
                boss.Nickname = value;
                NotifyPropertyChanged();
            }
        }

        public string Image 
        {
            get { return boss.Image; }
            set
            {
                boss.Image = value;
                NotifyPropertyChanged();
            }
        }

        public string Description 
        {
            get { return boss.Description; }
            set
            {
                boss.Description = value;
                NotifyPropertyChanged();
            }
        }

        public string Location 
        {
            get { return boss.Location; }
            set
            {
                boss.Location = value;
                NotifyPropertyChanged();
            }
        }

        public string[] Drops 
        {
            get { return boss.Drops; }
            set
            {
                boss.Drops = value;
                NotifyPropertyChanged();
            }
        }

        public string HealthPoints
        {
            get { return boss.HealthPoints; }
            set
            {
                boss.HealthPoints = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        // Gave ChatGpt some code and asked it "for some reason, going to a
        // different page and then coming back will not prevent me from
        // adding the same boss into the list" and it gave me these two
        // methods to solve my issue

        // Override Equals to compare based on unique properties
        public override bool Equals(object obj)
        {
            if (obj is BossViewModel otherBoss)
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
