using System;

namespace WM.GUID.Domain
{
    public class GuidMetadata
    {
        public GuidMetadata(string id, long? expire, string user, bool? isDeleted = false)
        {
            DateTime dateTime = DateTime.UtcNow;
            DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime.ToLocalTime());
            Id = id ?? System.Guid.NewGuid().ToString("N").ToUpper();
            Expire = expire ?? dateTimeOffset.AddDays(30).ToUnixTimeSeconds();
            IsDeleted = isDeleted;
            if (!string.IsNullOrEmpty(user))
                User = user;
            else
                throw new ArgumentNullException("Username cannot be null or empty");
        }

        private string _Id;
        public string Id
        {
            get => _Id;
            private set
            {             
                try
                {
                    _Id = new Guid(value).ToString("N").ToUpper();
                    Guid newGuid = System.Guid.Parse(_Id);
                }
                catch (ArgumentNullException)
                {
                    throw new ArgumentNullException(string.Format("The string to be parsed is null."));
                }
                catch (FormatException)
                {
                    throw new FormatException(string.Format("Bad format: {0}", _Id));
                }

            }
        }

        public long? Expire { get; private set; }

        public string User { get; private set; }

        public bool? IsDeleted { get; set; }
    }
}
