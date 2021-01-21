using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFOperator.Commands;
using WPFOperator.Models;

namespace WPFOperator.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private string ReservedEmployerName;
        private EmployerObject employer;
        private CardObject card;
        private Dictionary<int, string> cardTypesDictionary;
        private ObservableCollection<string> cardTypesList;
        private ObservableCollection<CardObject> cards;
        private ObservableCollection<EmployerObject> employers;
        private ObservableCollection<CardActionState> monthlyActions;
        private SaveManager manager;
        private LauncherKeyObject key;
        private bool isChildWindowClosed;
        private ICommand childWindowCommand;
        
        public EmployerObject Employer { get { return employer; } set { employer = value; OnPropertyChanged("Employer"); } }
        public CardObject Card { get { return card; } set { card = value; OnPropertyChanged("Card"); } }
        public ObservableCollection<CardObject> Cards { get { return cards; } set { cards = value; OnPropertyChanged("Cards"); } }
        public ObservableCollection<string> CardTypesList { get { return cardTypesList; } set { cardTypesList = value; OnPropertyChanged("CardTypesList"); }  }
        public ObservableCollection<EmployerObject> Employers { get { return employers; } set { employers = value; OnPropertyChanged("Employers"); } }
        public ObservableCollection<CardActionState> MonthlyActions { get { return monthlyActions; } set { monthlyActions = value; OnPropertyChanged("MonthlyActions"); } }
        public bool IsChildWindowClosed { get { return isChildWindowClosed; } set { isChildWindowClosed = value; OnPropertyChanged("IsChildWindowClosed"); } }
        public ICommand ChildWindowCommand { get { if (childWindowCommand == null) childWindowCommand = new RelayCommand(SetChildWindowClosed, CanExecute); return childWindowCommand; } }


        public MainViewModel()
        {
            manager = new SaveManager();
            Employers = manager.LoadEmployers();
            cardTypesDictionary = manager.LoadTypes();
            key = manager.LoadKey();
            IsChildWindowClosed = true;

            cardTypesList = new ObservableCollection<string>();
            Cards = new ObservableCollection<CardObject>();

            foreach (EmployerObject EO in Employers)
            {
                foreach (CardObject CO in EO.Cards)
                {
                    Cards.Add(CO);
                }
            }

            for (int i = 1; i < cardTypesDictionary.Count + 1; i++)
                cardTypesList.Add(cardTypesDictionary[i]);

            CardTypesList.Add("- - -");

            /*foreach (EmployerObject EO in Employers)
            {
                if (EO.FullName == "Lorein")
                {
                    Employer = EO;
                }
            }
            CardObject CO1 = new CardObject() { AddDate = new DateTime(2020, 9, 21), EmployerName = "Lorein", Number = "00000", CardType = "БЭПК" };
            CO1.RemoveDate = new DateTime(2020, 9, 23);
            CO1.Removed = true;

            Employer.AddCard(CO1);
            SaveEmployers();*/
            /*CardObject CO1 = new CardObject() { AddDate = new DateTime(2020, 9, 1), EmployerName = "Lorein", Number = "11111", CardType = "БЭПК" };
            CardObject CO2 = new CardObject() { AddDate = new DateTime(2020, 9, 11), EmployerName = "Lorein", Number = "22222", CardType = "БЭПК" };
            CardObject CO3 = new CardObject() { AddDate = new DateTime(2020, 10, 1), EmployerName = "Patrik", Number = "33333", CardType = "БЭПК" };
            CardObject CO4 = new CardObject() { AddDate = new DateTime(2020, 10, 11), EmployerName = "Patrik", Number = "44444", CardType = "БЭПК" };

            EmployerObject EO1 = new EmployerObject() { FullName = "Lorein" };
            EmployerObject EO2 = new EmployerObject() { FullName = "Patrik" };
            EO1.AddCard(CO1);
            EO1.AddCard(CO2);
            EO2.AddCard(CO3);
            EO2.AddCard(CO4);
            Cards.Add(CO1);
            Cards.Add(CO2);
            Cards.Add(CO3);
            Cards.Add(CO4);

            Employers.Add(EO1);
            Employers.Add(EO2);
            SaveEmployers();
            AddNewType("БЭПК");
            SaveTypes();*/
            /*try
            {
                BinaryFormatter bf = new BinaryFormatter();
                Stream s = new FileStream("Operators.opt", FileMode.OpenOrCreate);
                ObservableCollection<EmployerObject> OC = (ObservableCollection<EmployerObject>)bf.Deserialize(s);
                if (OC != null)
                {
                    Employers = OC;
                    foreach (EmployerObject EO in OC)
                    {
                        foreach (CardObject CO in EO.Cards)
                        {
                            Cards.Add(CO);
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }*/
        }

        public void SetReservedEmployerName()
        {
            ReservedEmployerName = Employer.FullName;
        }

        public void RestoreEmployerName()
        {
            Employer.FullName = ReservedEmployerName;
        }

        public void AddNewEmployer(string n)
        {
            EmployerObject EO = new EmployerObject();
            //EO.OnPropertyChanged("-");
            EO.Id = Employers.Count + 1;
            EO.FullName = n;
            Employers.Add(EO);

            SaveEmployers();
        }

        public void AddNewCard(string n, string t, DateTime dt)
        {
            CardObject CO = new CardObject() { CardType = t, Number = n, EmployerName = Employer.FullName };
            Employer.AddCard(CO, dt);
            Cards.Add(CO);

            SaveEmployers();
        }

        public void AddNewType(string t)
        {
            cardTypesDictionary.Add(cardTypesDictionary.Count + 1, t);
            cardTypesList.Clear();
            CardTypesList.Add("- - -");
            for (int i = 1; i < cardTypesDictionary.Count + 1; i++)
                cardTypesList.Add(cardTypesDictionary[i]);

            SaveTypes();
        }

        public void RemoveEmployer()
        {
            Employers.Remove(Employer);
            SaveEmployers();
        }

        public void RemoveCard(DateTime date)
        {
            Employer.RemoveCard(Card, date);
            //Card.RemoveCard(date);
            SaveEmployers();
        }

        public void ReturnCard(DateTime date)
        {
            Employer.ReturnCard(Card, date);
            //Card.ReturnCard(date);
            SaveEmployers();
        }

        public bool DeleteCard()
        {
            CardObject CO = Card;
            if (Employer.DeleteCard(CO) && Cards.Remove(CO))
            {
                Card = null;
                SaveEmployers();
                return true;
            }
            return false;
        }

        public void SelectEmployer(EmployerObject EO)
        {
            Employer = EO;
        }

        public void DeselectEmployer()
        {
            Employer = null;
        }

        public void SelectCard(CardObject CO)
        {
            Card = CO;
        }

        public void DeselectCard()
        {
            Card = null;
        }

        private void SetChildWindowClosed(object o)
        {
            IsChildWindowClosed = !isChildWindowClosed;
        }

        private bool CanExecute(object o)
        {
            return true;
        }

        public string IsCardExist(string n)
        {
            foreach (CardObject CO in Cards)
            {
                if (CO.Number == n && CO.EmployerName != "")
                {
                    return CO.EmployerName;
                }
            }

            return "";
        }

        public bool IsCardRemoved()
        {
            return Card.IsCardRemoved();
        }

        public bool IsCardReturned()
        {
            return Card.IsCardReturned();
        }

        public string GetCardNumber()
        {
            return Card.Number;
        }

        public void TransferCardFrom(string cardNumber, string EmployerFrom, DateTime date)
        {
            foreach (CardObject CO in Cards)
            {
                if (CO.Number == cardNumber)
                {
                    Card = CO;
                    break;
                }
            }

            foreach (EmployerObject EOFrom in Employers)
            {
                if (EOFrom.FullName == EmployerFrom)
                {
                    if (!Card.IsCardReturned())
                    {
                        EOFrom.ReturnCard(Card, date);
                        //Card.ReturnCard(date);
                    }
                    
                    Employer.AddCard(Card, date);
                    //Card.AddAction(date);
                    Card.EmployerName = Employer.FullName;
                    SaveEmployers();
                    break;
                }
            }

            Card = null;
        }

        public void FormActionsData(DateTime bgn, DateTime end)
        {
            foreach (EmployerObject EO in Employers)
            {
                EO.GetActionsState(bgn, end);
            }
            /*foreach (EmployerObject EO in Employers)
            {
                EO.SelectedDateState = new CardActionState();
                foreach (CardObject CO in EO.Cards)
                {
                    if (CO.AddDate.Year <= selectedDate.Year &&
                        CO.AddDate.Month <= selectedDate.Month)
                    {
                        if (CO.AddDate.Day < selectedDate.Day)
                        {
                            EO.SelectedDateState.StartCount++;
                            EO.SelectedDateState.LeftCount++;
                        }
                        
                        if (CO.RemoveDate.Day < selectedDate.Day)
                        {
                            EO.SelectedDateState.StartCount--;
                            EO.SelectedDateState.LeftCount--;
                        }

                        if (CO.AddDate.Day == selectedDate.Day)
                        {
                            EO.SelectedDateState.TakenCount++;
                            EO.SelectedDateState.LeftCount++;
                        }
                        
                        if (CO.RemoveDate.Day == selectedDate.Day)
                        {
                            EO.SelectedDateState.GivenCount++;
                            EO.SelectedDateState.LeftCount--;
                        }
                        
                    }
                }
            }*/
        }

        public void SaveEmployers()
        {
            manager.SaveEmployers(Employers);
        }

        public void SaveTypes()
        {
            manager.SaveCardTypes(cardTypesDictionary);
        }

        public void RemoveMonthlyActions()
        {
            MonthlyActions = null;
        }

        public bool IsKeyUptoDate()
        {
            return key.CheckUpdated();
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged(string pn)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pn));
            }
        }
    }
}
