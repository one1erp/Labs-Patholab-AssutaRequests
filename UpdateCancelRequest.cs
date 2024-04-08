using Patholab_DAL_V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssutaRequests
{
    class UpdateCancelRequest
    {
        private Patholab_DAL_V1.DataLayer _dal;
        private Patholab_DAL_V1.PHRASE_HEADER InterfaceParams;
        private bool updateYear;
        private U_SAMPLE_MSG_USER SMU;

        public UpdateCancelRequest(Patholab_DAL_V1.DataLayer _dal, Patholab_DAL_V1.PHRASE_HEADER InterfaceParams)
        {
            this._dal = _dal;
            this.InterfaceParams = InterfaceParams;
        }

        internal void Run()
        {
            try
            {
                Program.log("**************************************\nFifth phase:Check over messages for cancel.");
                DateTime date = DateTime.Today.AddMonths(-2);
                var msg2cancelSdg = _dal.GetAll<U_SAMPLE_MSG_USER>()
                  .Where(x => (x.U_STATUS == "C" || x.U_STATUS == "U") && x.U_ORDER_ID.HasValue && x.U_EXECUTE_TIME >= date);
                Program.log(msg2cancelSdg.Count() + " Messages Found from the past 2 months.");

                foreach (var item in msg2cancelSdg)
                {
                    SMU = item;
                    var newSDg = _dal.FindBy<SDG>(x => x.EXTERNAL_REFERENCE == item.U_REQUEST_NUM
                        && (x.STATUS == "V" || x.STATUS == "U")).OrderByDescending(x => x.SDG_ID).FirstOrDefault();

                    if (newSDg != null)
                    {
                        if (item.U_STATUS == "C")
                        {
                            Program.log(string.Format("Cancel {0}-{1} by  Message {2} ", newSDg.NAME, newSDg.EXTERNAL_REFERENCE, item.U_SAMPLE_MSG_ID));
                            newSDg.STATUS = "X";
                            newSDg.DESCRIPTION = string.Format(" {0}-{1} Canceled by  Message {2} ", newSDg.NAME, newSDg.EXTERNAL_REFERENCE, item.U_SAMPLE_MSG_ID);
                            item.U_STATUS = "X";
                        }
                        else
                        {
                            Program.log(string.Format("Update {0}-{1} by  Message {2} ", newSDg.NAME, newSDg.EXTERNAL_REFERENCE, item.U_SAMPLE_MSG_ID));
                            newSDg.STATUS = "S";
                            newSDg.DESCRIPTION = string.Format(" {0}-{1} Updated by  Message {2} ", newSDg.NAME, newSDg.EXTERNAL_REFERENCE, item.U_SAMPLE_MSG_ID);
                            item.U_STATUS = "D";
                        }
                        
                    }

                    _dal.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Program.log(SMU != null ? "Error while working on SAMPLE MSG USER number : " + SMU.U_REQUEST_NUM : "Error in UpdateCancelRequest query. ");
                Program.log("THE ERROR IS: " + ex + ". Inner error is : " + ex.InnerException);
            }

        }
    }
}
