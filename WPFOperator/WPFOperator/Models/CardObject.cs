using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFOperator.Models
{
    [Serializable]
    class CardObject : INotifyPropertyChanged
    {
        private string cardType;
        private string number;
        private string employerName;
        private ObservableCollection<CardAction> actions;
        private DateTime addedLastTime;
        private DateTime removedLastTime;
        private DateTime returnedLastTime;
        /*private DateTime addDate;
        private DateTime removedDate;
        private DateTime backDate;
        private bool removed;*/

        public string CardType { get { return cardType; } set { cardType = value; OnPropertyChanged("CardType"); } }
        public string Number { get { return number; } set { number = value; OnPropertyChanged("Number"); } }
        public string EmployerName { get { return employerName; } set { employerName = value; OnPropertyChanged("EmployerName"); } }
        public ObservableCollection<CardAction> Actions { get { return actions; } set { actions = value; OnPropertyChanged("Actions"); } }
        public DateTime AddedLastTime { get { return addedLastTime; } set { addedLastTime = value; OnPropertyChanged("AddedLastTime"); } }
        public DateTime RemovedLastTime { get { return removedLastTime; } set { removedLastTime = value; OnPropertyChanged("RemovedLastTime"); } }
        public DateTime ReturnedLastTime { get { return returnedLastTime; } set { returnedLastTime = value; OnPropertyChanged("ReturnedLastTime"); } }
        public string AddedLastTimeToString { get { if ( AddedLastTime.Year < 1900) return "- - -"; return AddedLastTime.ToString("yyyy/MM/dd"); } }
        public string RemovedLastTimeToString { get { if ( RemovedLastTime.Year < 1900) return "- - -"; return RemovedLastTime.ToString("yyyy/MM/dd"); } }
        public string ReturnedLastTimeToString { get { if ( ReturnedLastTime.Year < 1900) return "- - -"; return ReturnedLastTime.ToString("yyyy/MM/dd"); } }

        /*public DateTime AddDate { get { return addDate; } set { addDate = value; OnPropertyChanged("AddDate"); } }
        public DateTime RemoveDate { get { return removedDate; } set { removedDate = value; OnPropertyChanged("RemovedDate"); } }
        public DateTime BackDate { get { return backDate; } set { backDate = value; OnPropertyChanged("BackDate"); } }
        public string AddDateShortString { get { return addDate.ToString("yyyy/MM/dd"); } }
        public string RemoveDateShortString { get { if (removed) return removedDate.ToString("yyyy/MM/dd"); return "- - -"; } }
        public string BackDateShortString { get { if (backDate.Year > 1900) return backDate.ToString("yyyy/MM/dd"); return "- - -"; } }
        public bool Removed { get { return removed; } set { removed = value; OnPropertyChanged("Removed"); } }*/
        //public string NumberType { get { return number + " - " + cardType; } }

        public CardObject()
        {
            Actions = new ObservableCollection<CardAction>();
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public void AddAction()
        {
            if (Actions.Count == 0 || Actions[Actions.Count - 1].GetActionType() != 0)
            {
                DateTime dt = DateTime.Now;
                AddedLastTime = new DateTime(dt.Year, dt.Month, dt.Day);
                CreateAction(AddedLastTime, 0);
            }
        }

        public void AddAction(DateTime date)
        {
            if (Actions.Count == 0 || Actions[Actions.Count - 1].GetActionType() != 0)
            {
                AddedLastTime = date;
                CreateAction(AddedLastTime, 0);
            }
        }

        public bool RemoveCard()
        {
            if (Actions[Actions.Count - 1].GetActionType() == 0)
            {
                DateTime dt = DateTime.Now;
                RemovedLastTime = new DateTime(dt.Year, dt.Month, dt.Day);
                CreateAction(RemovedLastTime, 1);
                return true;
            }
            return false;
        }

        public bool RemoveCard(DateTime date)
        {
            if (Actions[Actions.Count - 1].GetActionType() == 0)
            {
                RemovedLastTime = date;
                CreateAction(RemovedLastTime, 1);
                return true;
            }
            return false;
        }

        public bool ReturnCard()
        {
            if (Actions[Actions.Count - 1].GetActionType() == 0)
            {
                DateTime dt = DateTime.Now;
                ReturnedLastTime = new DateTime(dt.Year, dt.Month, dt.Day);
                CreateAction(ReturnedLastTime, 2);
                return true;
            }
            return false;
        }

        public bool ReturnCard(DateTime date)
        {
            if (Actions[Actions.Count - 1].GetActionType() == 0)
            {
                ReturnedLastTime = date;
                CreateAction(ReturnedLastTime, 2);
                return true;
            }
            return false;
        }

        private void CreateAction(DateTime date, int act)
        {
            Actions.Add(new CardAction(date, act));
        }

        public bool IsAddedBetween(DateTime bgn, DateTime end)
        {
            foreach (CardAction CA in Actions)
            {
                if (bgn <= CA.GetDate() && CA.GetDate() <= end)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsCardRemoved()
        {
            return Actions[Actions.Count - 1].GetActionType() == 1;
        }

        public bool IsCardReturned()
        {
            return Actions[Actions.Count - 1].GetActionType() == 2;
        }

        protected void OnPropertyChanged(string pn)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pn));
            }
        }

        public override string ToString()
        {
            if (IsCardRemoved() || IsCardReturned() || EmployerName == "")
            {
                return number + " - " + cardType + " Не в наличии";
            }
            return number + " - " + cardType;
        }
    }
}
