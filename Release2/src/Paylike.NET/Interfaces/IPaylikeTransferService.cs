using Paylike.NET.Entities;
using Paylike.NET.RequestModels.Transactions;
using Paylike.NET.ResponseModels;

namespace Paylike.NET
{
    internal interface IPaylikeTransferService
    {
        ApiResponse<Transfer> CreateTransfer(CreateTransferRequest request);
        ApiResponse<Transfer> FetchTransfer(FetchTransferRequest request);
        ApiResponse<Transfer> ApproveTransfer(ApproveTransferRequest request);
    }
}