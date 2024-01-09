using AssetLoaningApplication.Data;
using AssetLoaningApplication.Models.Domain;
using AssetLoaningApplication.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace AssetLoaningApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly TransactionService transactionService;
        public TransactionController(TransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        public AssetLoanDbContext DbContext { get; }

        /// <summary>
        /// It is a post API which takes type as url param and based on the type like loan or return it performs the add transaction.
        /// </summary>
        /// <param name="loanAssetTransaction">It takes input json body which consists Requesting user id , loaning or receiving supervisor id , student id , asset id and the date</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{type}")]
        public IActionResult addTransaction(string type,AddTransaction loanAssetTransaction)
        {
            try
            {
                var result = transactionService.AddTransaction(type,loanAssetTransaction);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, $"{ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(404, $"{ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, $"{ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500,$"{ex.Message}");
            }
           
        }


        /// <summary>
        /// It is a put API which takes type as url param and based on the type like loan or return it performs the update transaction.
        /// </summary>
        /// <param name="loanAssetTransaction">It takes input json body which consists Requesting user id , transaction id, loaning or receiving supervisor id , student id , asset id and the date</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{type}")]
        public IActionResult updateTransaction(string type, UpdateTransaction loanAssetTransaction)

        {
            try
            {
                var result = transactionService.UpdateTransaction(type, loanAssetTransaction);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(400, $"{ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(400, $"{ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(404, $"{ex.Message}");
            }
           
        }


        /// <summary>
        /// It is a get API which takes type as url param and based on the type like loan or return and get the details of particular transaction.
        /// </summary>
        /// <param name="getLoanTransactionData">It takes requesting user id and transaction id as string query parameter.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{type}")]
        public IActionResult getTransaction(string type, [FromQuery] Guid requestingUserId, [FromQuery] Guid transactionId)
        {
            try
            {
                var result = transactionService.GetTransaction(type, requestingUserId, transactionId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(400, $"{ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(400, $"{ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(404, $"{ex.Message}");
            }

       
        }

        
        
        
        /// <summary>
        /// It is a get API which returns the details of the transaction based on filters.
        /// </summary>
        /// <param name="filterTransaction">I takes requesting user Id as a string query and supervisor id , student id , asset id , and date as filters.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Filter")]
        public IActionResult getByFilter([FromQuery] Guid requestingUserId, [FromQuery] FilterTransaction filterTransaction)
        {
            try
            {
                var result = transactionService.GetByFilter(requestingUserId, filterTransaction);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(400, $"{ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(400, $"{ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(404, $"{ex.Message}");
            }

        }


   

    }
}
