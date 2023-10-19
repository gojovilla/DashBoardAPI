﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashBoardAPI
{
    public class BaseEntity
    {
        #region Properties

        public Int64 Id { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }

        public Int64 CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }


        public Int64 ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

      //  public Int64 TotalCount { get; set; }

      //  public Int64 CurrentUserRoleID { get; set; }

      //  public string? EncryptedParam { get; set; }

        //public string Roles { get; set; }

        
     /// <summary>
     /// /  public int LogoutTime { get; set; }
     /// </summary>
    //    public Int64 BintEmployeeID { get; set; }
        #endregion
    }
}
