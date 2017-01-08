using System;
using System.Linq;

namespace NGMP.WPF
{
    public class StatusBarAccessor : ANotifyBase, IComparable<StatusBarAccessor>, IEquatable<StatusBarAccessor>
    {
        /// <summary>
        ///     The max amount of messages that will be displayed at once. Once this number is reached, older messages will be
        ///     deleted.
        /// </summary>
        private const int MaxMessages = 60;

        private int _messageCount;

        #region List Properties

        /// <summary>
        ///     The list of messages, which gets bound to the StatusStyle combobox
        /// </summary>
        public AsyncObservableCollection<StatusBarAccessor> StatusList
        {
            get { return Get(() => StatusList); }
            set { Set(value, () => StatusList); }
        }

        /// <summary>
        ///     The currently shown stats
        /// </summary>
        public StatusBarAccessor SelectedStatus
        {
            get { return Get(() => SelectedStatus); }
            set { Set(value, () => SelectedStatus); }
        }

        #endregion List Properties

        #region Object Properties

        /// <summary>
        ///     The string representation of the MessageStatus enum
        /// </summary>
        public string MessageStatusString
        {
            get { return this.MessageStatus.ToString(); }
        }

        /// <summary>
        ///     The importance level of the message
        /// </summary>
        public StatusEnum MessageStatus
        {
            get { return Get(() => MessageStatus); }
            set { Set(value, () => MessageStatus); }
        }

        public string MissedStatus
        {
            get { return Get(() => MissedStatus); }

            set { Set(value, () => MissedStatus); }
        }

        /// <summary>
        ///     The status message text
        /// </summary>
        public string Message
        {
            get { return Get(() => Message); }
            set { Set(value, () => Message); }
        }

        /// <summary>
        ///     The message id.
        ///     Increases with every message, used to delete older messages.
        /// </summary>
        public int MessageId
        {
            get { return Get(() => MessageId); }
            private set { Set(value, () => MessageId); }
        }

        /// <summary>
        ///     The time at which the message was logged.
        /// </summary>
        public DateTime MessageTime
        {
            get { return Get(() => MessageTime); }
            private set { Set(value, () => MessageTime); }
        }

        #endregion Object Properties

        /// <summary>
        ///     Adds a message to the message list and displays it as the most current one.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="status">The state of the message. Info level is the default</param>
        public void Add(string message, StatusEnum status = StatusEnum.Info)
        {
            lock (this)
            {
                StatusBarAccessor sba = new StatusBarAccessor();
                sba.Message = message;
                sba.MessageStatus = status;
                sba.MessageId = ++_messageCount;
                sba.MessageTime = DateTime.Now;

                if (this.StatusList == null)
                {
                    this.StatusList = new AsyncObservableCollection<StatusBarAccessor>();
                }

                this.StatusList.Remove(x => x.MessageStatus == StatusEnum.Debug);

                if (this.StatusList.Any())
                {
                    var lastMessage = this.StatusList.SingleOrDefault(y => y.MessageId == this.StatusList.Max(x => x.MessageId));

                    if (lastMessage != null)
                    {
                        if (lastMessage.MessageStatus != StatusEnum.Info &&
                            sba.MessageStatus != StatusEnum.Info &&
                            (sba.MessageTime - lastMessage.MessageTime).TotalMilliseconds < 2000)
                        {
                            this.MissedStatus = sba.MessageStatusString;
                        }
                        else
                        {
                            if (lastMessage.MissedStatus == StatusEnum.Error.ToString() || lastMessage.MissedStatus == StatusEnum.Warn.ToString())
                            {
                                this.MissedStatus = this.MissedStatus != StatusEnum.Error.ToString() ? lastMessage.MissedStatus : this.MissedStatus;
                            }
                            else
                            {
                                this.MissedStatus = String.Empty;
                            }
                        }
                    }
                }

                sba.MissedStatus = this.MissedStatus;
                this.StatusList.Add(sba);

                this.StatusList.SortDesc();

                if ((this.StatusList.Count > MaxMessages) && (MaxMessages > 0))
                {
                    StatusBarAccessor temp = this.StatusList.SingleOrDefault(y => y.MessageId == this.StatusList.Min(x => x.MessageId));
                    this.StatusList.Remove(temp);
                }

                this.SelectedStatus = sba;
            }
        }

        #region Interface Implementation

        public int CompareTo(StatusBarAccessor that)
        {
            return this.MessageId.CompareTo(that.MessageId);
        }

        public bool Equals(StatusBarAccessor other)
        {
            if (other == null) return false;
            return (this.MessageId.Equals(other.MessageId));
        }

        #endregion Interface Implementation

    }

    public enum StatusEnum
    {
        Info,
        Warn,
        Error,
        Debug
    }
}