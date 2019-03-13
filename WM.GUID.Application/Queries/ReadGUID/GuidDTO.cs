using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WM.GUID.Domain;

namespace WM.GUID.Application.Queries.ReadGUID
{
    public class GuidDTO
    {
        public string GUID { get; set; }

        public long Expire { get; set; }

        public string User { get; set; }

        public static Expression<Func<GuidMetadata, GuidDTO>> Projection
        {
            get
            {
                return guid => new GuidDTO
                {
                    GUID = guid.Id,
                    Expire  = Convert.ToInt64(guid.Expire),
                    User = guid.User
                };
            }
        }

        public static GuidDTO Create(GuidMetadata metadata)
        {
            return Projection.Compile().Invoke(metadata);
        }
    }
}
