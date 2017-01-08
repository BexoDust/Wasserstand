using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wasserstand.Model;
using Wasserstand.Parsing;
using Windows.Storage;
using Windows.Storage.Pickers;
using WpfHelper;
using XmlReadWriteUniversal;
using static Wasserstand.Model.Enums;

namespace Wasserstand.ViewModel
{
    public class BasicController : ANotifyBase
    {
        private List<IParsing> _parserList;
        private string _transactionSave = @"List.xml";
        private string _tagSave = @"Tags.xml";
        private List<TransactionEntry> _completeTransactionList;

        #region Properties
        public string TransactionPath
        {
            get { return Get(() => TransactionPath); }
            set { Set(value, () => TransactionPath); }
        }

        public BankType Bank
        {
            get { return Get(() => Bank); }
            set { Set(value, () => Bank); }
        }


        public ObservableCollection<TransactionEntry> TransactionList
        {
            get { return Get(() => TransactionList); }
            set { Set(value, () => TransactionList); }
        }


        public TransactionEntry SelectedTransaction
        {
            get { return Get(() => SelectedTransaction); }
            set { Set(value, () => SelectedTransaction); }
        }


        public ObservableCollection<Tag> TagList
        {
            get { return Get(() => TagList); }
            set { Set(value, () => TagList); }
        }


        public Tag SelectedTag
        {
            get { return Get(() => SelectedTag); }
            set { Set(value, () => SelectedTag);
                OnPropertyChanged("TagList");
            }
        }
        
        public ObservableCollection<TagTotal> IncomeTagOverview
        {
            get { return Get(() => IncomeTagOverview); }
            set { Set(value, () => IncomeTagOverview); }
        }

        public ObservableCollection<TagTotal> OutcomeTagOverview
        {
            get { return Get(() => OutcomeTagOverview); }
            set { Set(value, () => OutcomeTagOverview); }
        }


        public DateTimeOffset StartTime
        {
            get { return Get(() => StartTime); }
            set { Set(value, () => StartTime); }
        }

        public DateTimeOffset EndTime
        {
            get { return Get(() => EndTime); }
            set { Set(value, () => EndTime); }
        }


        public bool ShowFromStart
        {
            get { return Get(() => ShowFromStart); }
            set { Set(value, () => ShowFromStart); }
        }


        public bool ShowTillEnd
        {
            get { return Get(() => ShowTillEnd); }
            set { Set(value, () => ShowTillEnd); }
        }

        #endregion Properties

        public BasicController()
        {
            this.Bank = BankType.Commerzbank;

            this._parserList = new List<IParsing>();
            this._parserList.Add(new CommerzbankParsing());

            this.StartTime = DateTime.Now;
            this.EndTime = DateTime.Now;
            this.ShowFromStart = true;
            this.ShowTillEnd = true;

            this.Init();
        }

        #region Commands

        public RelayCommand LoadTransactionCommand
        {
            get
            {
                if (Get(() => LoadTransactionCommand) == null)
                    Set(new RelayCommand(() => this.LoadTransaction()), () => LoadTransactionCommand);

                return Get(() => LoadTransactionCommand);
            }
        }

        public RelayCommand AddTagCommand
        {
            get
            {
                if (Get(() => AddTagCommand) == null)
                    Set(new RelayCommand(() => this.AddTag()), () => AddTagCommand);

                return Get(() => AddTagCommand);
            }
        }

        public RelayCommand RemoveTagCommand
        {
            get
            {
                if (Get(() => RemoveTagCommand) == null)
                    Set(new RelayCommand(() => this.RemoveTag(), () => this.CanRemoveTag()), () => RemoveTagCommand);

                return Get(() => RemoveTagCommand);
            }
        }

        public RelayCommand MoveTagUpCommand
        {
            get
            {
                if (Get(() => MoveTagUpCommand) == null)
                    Set(new RelayCommand(() => this.MoveTagUp(), () => this.CanMoveTagUp()), () => MoveTagUpCommand);

                return Get(() => MoveTagUpCommand);
            }
        }

        public RelayCommand MoveTagDownCommand
        {
            get
            {
                if (Get(() => MoveTagDownCommand) == null)
                    Set(new RelayCommand(() => this.MoveTagDown(), () => this.CanMoveTagDown()), () => MoveTagDownCommand);

                return Get(() => MoveTagDownCommand);
            }
        }

        public RelayCommand SaveTagCommand
        {
            get
            {
                if (Get(() => SaveTagCommand) == null)
                    Set(new RelayCommand(() => this.SaveTagsToXml()), () => SaveTagCommand);

                return Get(() => SaveTagCommand);
            }
        }

        public RelayCommand UpdateDateCommand
        {
            get
            {
                if (Get(() => UpdateDateCommand) == null)
                    Set(new RelayCommand(() => this.UpdateDate(), () => this.CanUpdateDate()), () => UpdateDateCommand);

                return Get(() => UpdateDateCommand);
            }
        }

        #endregion Commands

        #region Methods

        private async void Init()
        {
            this.TagList = new ObservableCollection<Tag>();
            this.LoadTransactionListFromXml();
            this.TagList = await this.LoadTagsFromXml();
            this.AnalyseList();
        }

        public async void LoadTransaction()
        {
            try
            {
                var picker = new FileOpenPicker();
                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".csv");

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    this.TransactionPath = file.Path;
                }

                var list = new List<TransactionEntry>(await this.GetParser().ReadTransactions(file));

                this.UpdateList(list);
                this._completeTransactionList = list;
                this.TransactionList = new ObservableCollection<TransactionEntry>(this._completeTransactionList);
                this.SaveTransactionListToXml();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void AddTag()
        {
            Tag newTag = new Tag();
            newTag.Name = "Please select a name";
            newTag.Type = TagType.Description;
            newTag.Order = this.TagList.Count > 0 ? this.TagList.Max(x => x.Order) + 1 : 1;

            this.TagList.Add(newTag);
        }

        private bool CanRemoveTag()
        {
            return this.SelectedTag != null;
        }

        private void RemoveTag()
        {
            this.TagList.Remove(this.SelectedTag);
        }

        private bool CanMoveTagUp()
        {
            return this.SelectedTag?.Order > 1;
        }

        private void MoveTagUp()
        {
            Tag upperTag = this.TagList.SingleOrDefault(x => x.Order == this.SelectedTag.Order - 1);
            this.SelectedTag.Order--;
            upperTag.Order++;

            this.TagList.Sort();
            this.OnPropertyChanged("TagList");
        }

        private bool CanMoveTagDown()
        {
            return this.SelectedTag?.Order < this.TagList.Max(x => x.Order);
        }

        private void MoveTagDown()
        {
            Tag lowerTag = this.TagList.SingleOrDefault(x => x.Order == this.SelectedTag.Order + 1);
            this.SelectedTag.Order++;
            lowerTag.Order--;

            this.TagList.Sort();
            this.OnPropertyChanged("TagList");
        }

        private IParsing GetParser()
        {
            IParsing result = this._parserList.Single(x => x.GetBankType() == this.Bank);

            return result;
        }

        private void UpdateList(List<TransactionEntry> transactionList)
        {
            transactionList.Sort();
            
            double currentTotal = 0;
            DateTime lastDate = DateTime.MinValue;

            foreach (var entry in transactionList)
            {
                currentTotal += entry.Amount;
                entry.Total = currentTotal;
            }
        }

        private async void SaveTransactionListToXml()
        {
            await XmlIo.SaveObjectToXml(this._completeTransactionList, this._transactionSave);
        }

        private async void LoadTransactionListFromXml()
        {
            if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, this._transactionSave)))
            {
                var list = await XmlIo.ReadObjectFromXmlFileAsync<ObservableCollection<TransactionEntry>>(this._transactionSave);
                this._completeTransactionList = new List<TransactionEntry>(list);
                this.TransactionList = list;
            }
        }

        public async void SaveTagsToXml()
        {
            await XmlIo.SaveObjectToXml(this.TagList, this._tagSave);
            this.AnalyseList();
        }

        private async Task<ObservableCollection<Tag>> LoadTagsFromXml()
        {
            if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, this._tagSave)))
            {
                return await XmlIo.ReadObjectFromXmlFileAsync<ObservableCollection<Tag>>(this._tagSave);
            }

            return null;
        }

        private void AnalyseList()
        {
            var tagList = new ObservableCollection<Tag>(this.TagList);
            tagList.SortDesc();

            foreach (var tag in tagList)
            {
                foreach (var entry in this.TransactionList)
                {
                    switch (tag.Type)
                    {
                        case TagType.Amount:
                            if (Math.Abs(entry.Amount) > tag.Amount)
                            {
                                entry.Tag = tag.Name;
                            }
                            break;
                        case TagType.Description:
                            if (entry.Description.ToUpper().Contains(tag.Description.ToUpper()))
                            {
                                entry.Tag = tag.Name;
                            }
                            break;
                        case TagType.Type:
                            if (entry.Type == tag.Transaction)
                            {
                                entry.Tag = tag.Name;
                            }
                            break;
                    }
                }
            }

            ObservableCollection<TagTotal> incomeTagOverview = new ObservableCollection<TagTotal>();
            ObservableCollection<TagTotal> outcomeTagOverview = new ObservableCollection<TagTotal>();

            foreach (var entry in this.TransactionList)
            {
                if (entry.Type == TransactionType.Gutschrift || entry.Type == TransactionType.Zinsen)
                {
                    var tag = incomeTagOverview.SingleOrDefault(x => x.Name == entry.Tag);
                    if (tag != null)
                    {
                        tag.Value += Math.Abs(entry.Amount);
                        entry.TagTotal = tag.Value;
                    }
                    else
                    {
                        incomeTagOverview.Add(new TagTotal() { Name = entry.Tag, Value = Math.Abs(entry.Amount) });
                        entry.TagTotal = Math.Abs(entry.Amount);
                    }
                }
                else
                {
                    var tag = outcomeTagOverview.SingleOrDefault(x => x.Name == entry.Tag);
                    if (tag != null)
                    {
                        tag.Value += Math.Abs(entry.Amount);
                        entry.TagTotal = tag.Value;
                    }
                    else
                    {
                        outcomeTagOverview.Add(new TagTotal() { Name = entry.Tag, Value = Math.Abs(entry.Amount) });
                        entry.TagTotal = Math.Abs(entry.Amount);
                    }
                }
            }
            
            this.IncomeTagOverview = incomeTagOverview;
            this.OutcomeTagOverview = outcomeTagOverview;
            this.SaveTransactionListToXml();
        }

        private bool CanUpdateDate()
        {
            return this.TransactionList != null;
        }

        private void UpdateDate()
        {
            List<TransactionEntry> filteredList = new List<TransactionEntry>();

            filteredList = this._completeTransactionList.Where(x => (x.Date < this.EndTime || this.ShowTillEnd) && (x.Date > this.StartTime || this.ShowFromStart)).ToList();

            this.TransactionList = new ObservableCollection<TransactionEntry>(filteredList);
            this.AnalyseList();
        }

        #endregion Methods
    }
}
