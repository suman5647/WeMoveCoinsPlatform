//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WMC.Data
//{
//    public class WorkFlowProcessor<T> where T : IWFObject
//    {
//        private T wfobj;
//        private WFState currentState;
//        private List<WFState> states;

//        public WorkFlowProcessor(T workflowObject)
//        {
//            this.wfobj = workflowObject;
//            currentState = default(WFState); // TODO
//            resolvecurrentstate();
//        }

//        private void resolvecurrentstate()
//        {
//            foreach (var st in WMC.Data.Validate.WFValidation.states)
//            {
//                bool matched = true;
//                var stenum = st.State.GetEnumerator();
//                foreach (var wfSt in wfobj.GetState())
//                {
//                    if ((long)stenum.Current != wfSt)
//                    {
//                        matched = false;
//                        break;
//                    }

//                    stenum.MoveNext();
//                }

//                if (matched) { currentState = st; break; }
//            }
//        }

//        public bool ExecuteAction(long userId, string actionName, params object[] actionParams)
//        {
//            if (!(currentState.Actions.Contains(actionName)))
//            {
//                // TODO: log "Action not allowed"
//                return false;
//            }

//            IWFAction<T> action = this.getAction(actionName);
//            action.Context = wfobj;
//            action.Context.UserId = userId;
//            if (action.Execute(userId, actionParams))
//            {
//                bool allowLatestState = false;
//                var unsavedState = action.Context.GetState();
//                foreach (var possibleState in currentState.NextStates)
//                {
//                    bool matched = true;
//                    var curenu = possibleState.GetEnumerator();
//                    foreach (var item in unsavedState)
//                    {
//                        if (matched && (long)curenu.Current != item)
//                        {
//                            matched = false;
//                            break;
//                        }
//                    }

//                    if (matched)
//                    {
//                        allowLatestState = true;
//                        break;
//                    }
//                }

//                return allowLatestState;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        static Dictionary<string, IWFAction<T>> actions = new Dictionary<string, IWFAction<T>>();
//        public static void RegisterAction(string actionName, IWFAction<T> action)
//        {
//            actions[actionName] = action;
//        }

//        private IWFAction<T> getAction(string actionName)
//        {
//            return actions[actionName]; // default(IWFAction<T>); // TODO
//        }
//    }

//    public interface IWFAction<T> where T : IWFObject
//    {
//        string ActionName { get; }
//        T Context { get; set; }
//        bool Execute(long userId, params object[] parameters);
//    }

//    public struct WFState
//    {
//        public bool AutoMove { get; set; }
//        public long[] State { get; set; }
//        public List<long[]> NextStates { get; set; }
//        public string[] Actions { get; set; }
//    }

//    public interface IWFObject : IEquatable<IWFObject>
//    {
//        string ReferenceId { get; set; }
//        long UserId { get; set; }
//        IEnumerable<long> GetState();
//        // void SetState(string stateName, long stateValue);
//    }

//    public abstract class OrderWFObject : IWFObject
//    {
//        public long Id { get; set; }
//        [Required]
//        [StringLength(8)]
//        public string Number { get; set; }
//        public long UserId { get; set; }

//        [NotMapped]
//        public string ReferenceId { get { return this.Number; } set { this.Number = value; } }
//        [NotMapped]
//        public Enums.OrderType Type { get; set; }
//        [NotMapped]
//        public Enums.OrderStatus Status { get; set; }
//        [NotMapped]
//        public Enums.OrderPaymentType PaymentType { get; set; }
//        public IEnumerable<long> GetState()
//        {
//            yield return (long)Type;
//            yield return (long)Status;
//            yield return (long)PaymentType;
//            yield break;
//        }

//        public bool Equals(IWFObject other)
//        {
//            var isEqual = other.ReferenceId.Equals(this.ReferenceId) && other.UserId.Equals(this.UserId);
//            if (!isEqual) return false;
//            var state1 = other.GetState().GetEnumerator();
//            var state2 = this.GetState().GetEnumerator();
//            isEqual = isEqual && state1.Current.Equals(state2.Current); // type comparison
//            if (!isEqual) return false;
//            state1.MoveNext();
//            state2.MoveNext();
//            isEqual = isEqual && state1.Current.Equals(state2.Current); // status comparison
//            if (!isEqual) return false;
//            state1.MoveNext();
//            state2.MoveNext();
//            isEqual = isEqual && state1.Current.Equals(state2.Current); // payment type comparison
//            return isEqual;
//        }
//    }
//}

//namespace WMC.Data.Validate
//{
//    public class WFValidation
//    {
//        static WFValidation()
//        {
//            WorkFlowProcessor<Order>.RegisterAction("OrderKYCAction", new OrderKYCAction());
//            WorkFlowProcessor<Order>.RegisterAction("OrderPayoutAction", new OrderPayoutAction());
//            WorkFlowProcessor<Order>.RegisterAction("OrderPayoutRelease", new OrderPayoutRelease());
//            WorkFlowProcessor<Order>.RegisterAction("OrderUserKYCAction", new OrderUserKYCAction());
//        }

//        public static WFState[] states = new WFState[]
//            {
//                new WFState()
//                {
//                    State = new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.Quoted, (long)Data.Enums.OrderPaymentType.Bank },
//                    Actions = new string[]{ "OrderKYCAction" },
//                    NextStates = new List<long[]>()
//                    {
//                        new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.KYCApprovalPending, (long)Data.Enums.OrderPaymentType.Bank },
//                        new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.KYCApproved, (long)Data.Enums.OrderPaymentType.Bank },
//                        // new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.ComplianceOfficerApproval, (long)Data.Enums.OrderPaymentType.Bank },
//                        new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.OrderCancelled, (long)Data.Enums.OrderPaymentType.Bank },
//                    }
//                },
//                 new WFState()
//                {
//                    State = new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.KYCApproved, (long)Data.Enums.OrderPaymentType.Bank },
//                    Actions = new string[]{ "OrderPayoutAction" },
//                    AutoMove = true,
//                    NextStates = new List<long[]>()
//                    {
//                        new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.PayoutApproved, (long)Data.Enums.OrderPaymentType.Bank },
//                    }
//                },
//                 new WFState()
//                {
//                    State = new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.PayoutApproved, (long)Data.Enums.OrderPaymentType.Bank },
//                    Actions = new string[]{ "OrderPayoutRelease" },
//                    NextStates = new List<long[]>()
//                    {
//                        new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.Completed, (long)Data.Enums.OrderPaymentType.Bank },
//                        new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.OrderCancelled, (long)Data.Enums.OrderPaymentType.Bank },
//                    }
//                },
//                 new WFState()
//                {
//                    State = new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.KYCApprovalPending, (long)Data.Enums.OrderPaymentType.Bank },
//                    Actions = new string[]{ "OrderUserKYCAction", "OrderKYCAction" },
//                    NextStates = new List<long[]>()
//                    {
//                        new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.PayoutAwaitsApproval, (long)Data.Enums.OrderPaymentType.Bank },
//                        new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.OrderCancelled, (long)Data.Enums.OrderPaymentType.Bank },
//                    }
//                },
//                 new WFState()
//                {
//                    State = new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.PayoutAwaitsApproval, (long)Data.Enums.OrderPaymentType.Bank },
//                    Actions = new string[]{ "OrderKYCAction" },
//                    NextStates = new List<long[]>()
//                    {
//                        new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.KYCApprovalPending, (long)Data.Enums.OrderPaymentType.Bank },
//                        new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.KYCApproved, (long)Data.Enums.OrderPaymentType.Bank },
//                        // new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.ComplianceOfficerApproval, (long)Data.Enums.OrderPaymentType.Bank },
//                        new long[] {(long)Data.Enums.OrderType.Buy, (long)Data.Enums.OrderStatus.OrderCancelled, (long)Data.Enums.OrderPaymentType.Bank },
//                    }
//                }
//            };
//    }

//    public class OrderKYCAction : IWFAction<Order>
//    {
//        public string ActionName => "OrderKYCAction";

//        public Order Context { get; set; }

//        public bool Execute(long userId, params object[] parameters)
//        {
//            if (userId > 1000) return false;
//            if (parameters != null && parameters.Length > 0)
//            {
//                if (parameters[0].ToString() == "KYCApprovalPending")
//                {
//                    // check reason is updated
//                    Context.Status = Enums.OrderStatus.KYCApprovalPending;
//                }
//                else if (parameters[0].ToString() == "KYCApproved")
//                {
//                    // check all kycs are approved
//                    Context.Status = Enums.OrderStatus.KYCApproved;
//                }
//                else if (parameters[0].ToString() == "OrderCancelled")
//                {
//                    // check reason is updated
//                    Context.Status = Enums.OrderStatus.OrderCancelled;
//                }
//            }

//            return true;
//        }
//    }

//    public class OrderPayoutAction : IWFAction<Order>
//    {
//        public string ActionName => "OrderPayoutAction";

//        public Order Context { get; set; }

//        public bool Execute(long userId, params object[] parameters)
//        {
//            // validate
//            if (userId == 1) Context.Status = Enums.OrderStatus.PayoutApproved;
//            return true;
//        }
//    }

//    public class OrderPayoutRelease : IWFAction<Order>
//    {
//        public string ActionName => "OrderPayoutRelease";

//        public Order Context { get; set; }

//        public bool Execute(long userId, params object[] parameters)
//        {
//            if (userId == 1) Context.Status = Enums.OrderStatus.PayoutApproved;
//            if (parameters != null && parameters.Length > 0)
//            {
//                if (parameters[0].ToString() == "OrderCancelled")
//                {
//                    // check reason is updated
//                    Context.Status = Enums.OrderStatus.OrderCancelled;
//                    return true;
//                }
//            }

//            Context.Status = Enums.OrderStatus.Completed;
//            return true;
//        }
//    }

//    public class OrderUserKYCAction : IWFAction<Order>
//    {
//        public string ActionName => "OrderUserKYCAction";

//        public Order Context { get; set; }


//        public bool Execute(long userId, params object[] parameters)
//        {
//            if (userId <= 1000) return false;
//            // check new kycs are uploaded
//            Context.Status = Enums.OrderStatus.PayoutAwaitsApproval;

//            return true;
//        }
//    }
//}
