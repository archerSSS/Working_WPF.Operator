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
    class EmployerObject : INotifyPropertyChanged
    {
        private int id;
        private string fullName;
        private CardActionState selectedDateState;
        private CardActionsHandler CAH;
        private ObservableCollection<CardObject> cards;
        private ObservableCollection<CardObject> handledCards;

        public int Id { get { return id; } set { id = value; } }
        public string FullName { get { return fullName; } set { fullName = value; OnPropertyChanged("FullName"); } }
        public CardActionState SelectedDateState { get { return selectedDateState; } set { selectedDateState = value; OnPropertyChanged("SelectedDateState"); } }
        public ObservableCollection<CardObject> Cards { get { return cards; } set { cards = value; OnPropertyChanged("Cards"); } }
        public ObservableCollection<CardObject> HandledCards { get { return handledCards; } set { handledCards = value; OnPropertyChanged("HandledCards"); } }

        public EmployerObject()
        {
            //actions = new ObservableCollection<CardActionState>();
            cards = new ObservableCollection<CardObject>();
            handledCards = new ObservableCollection<CardObject>();
            CAH = new CardActionsHandler();
        }

        public void AddCard(CardObject CO)
        {
            DateTime dt = DateTime.Now;
            CAH.AddTimes.Add(new DateTime(dt.Year, dt.Month, dt.Day));
            CO.AddAction();
            Cards.Add(CO);
            HandledCards.Add(CO);
        }

        public void AddCard(CardObject CO, DateTime date)
        {
            CAH.AddTimes.Add(date);
            CO.AddAction(date);
            Cards.Add(CO);
            HandledCards.Add(CO);
        }

        public void RemoveCard(CardObject CO, DateTime date)
        {
            CAH.RemoveTimes.Add(date);
            HandledCards.Remove(CO);
            CO.RemoveCard(date);
            //DateTime newDate = DateTime.Now;
            /*if (Actions[0].Date.ToString("yyyy-MM-dd") == newDate.ToString("yyyy-MM-dd"))
            {
                CardActionState cas = Actions[0];
                cas.GivenCount++;
                cas.LeftCount--;
            }
            else
            {
                CardActionState cas = new CardActionState()
                {
                    Date = newDate,
                    StartCount = Cards.Count,
                    GivenCount = 1,
                    LeftCount = Cards.Count - 1
                };
                Actions.Insert(0, cas);
            }*/
            //cards.Remove(CO);
            /*DateTime dt = DateTime.Now;
            dt = new DateTime(dt.Year, dt.Month, dt.Day);

            CO.RemoveDate = dt;
            CO.Removed = true;*/
        }

        public void ReturnCard(CardObject CO, DateTime date)
        {
            CAH.ReturnTimes.Add(date);
            HandledCards.Remove(CO);
            CO.ReturnCard(date);
        }

        public bool DeleteCard(CardObject CO)
        {
            if (CO.Actions.Count < 2)
            {
                DateTime dt = CO.Actions[0].GetDate();
                Cards.Remove(CO);
                HandledCards.Remove(CO);
                CAH.DeleteAddAction(dt);
                return true;
            }
            return false;
        }

        public CardActionState GetActionsState(DateTime bgn, DateTime end)
        {
            CardActionState CAS = new CardActionState();
            foreach (DateTime AT in CAH.AddTimes)
            {
                if (AT < bgn)
                {
                    CAS.StartCount++;
                    CAS.LeftCount++;
                }
                else if (AT >= bgn && AT <= end)
                {
                    CAS.AddedCount++;
                    CAS.LeftCount++;
                }
            }

            foreach (DateTime RMT in CAH.RemoveTimes)
            {
                if (RMT < bgn)
                {
                    CAS.StartCount--;
                    CAS.LeftCount--;
                }
                else if (RMT >= bgn && RMT <= end)
                {
                    CAS.RemovedCount++;
                    CAS.LeftCount--;
                }
            }

            foreach (DateTime RTT in CAH.ReturnTimes)
            {
                if (RTT < bgn)
                {
                    CAS.StartCount--;
                    CAS.LeftCount--;
                }
                else if (RTT >= bgn && RTT <= end)
                {
                    CAS.ReturnedCount++;
                    CAS.LeftCount--;
                }
            }
            /*foreach (CardObject CO in Cards)
            {
                foreach (CardAction CA in CO.Actions)
                {
                    if (CA.GetDate() < bgn)
                    {
                        if (CA.GetActionType() == 0)
                        {
                            CAS.StartCount++;
                            CAS.LeftCount++;
                        }
                        else if (CA.GetActionType() == 1 || CA.GetActionType() == 2)
                        {
                            CAS.StartCount--;
                            CAS.LeftCount--;
                        }
                    }

                    if (bgn < CA.GetDate() && CA.GetDate() < end)
                    {
                        if (CA.GetActionType() == 0 && CO.EmployerName == FullName)
                        {
                            CAS.AddedCount++;
                            CAS.LeftCount++;
                        }
                        else if (CA.GetActionType() == 1)
                        {
                            CAS.RemovedCount++;
                            CAS.LeftCount--;
                        }
                        else if (CA.GetActionType() == 2)
                        {
                            CAS.ReturnedCount++;
                            CAS.LeftCount--;
                        }
                    }
                }
            }*/
            SelectedDateState = CAS;
            return CAS;
        }



        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        
        public void OnPropertyChanged(string pn)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pn));
            }
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
