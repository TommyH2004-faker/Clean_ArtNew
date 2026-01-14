using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Features.OrderHandle.Command.Cancel;
using TodoApp.Application.Features.OrderHandle.Command.Confirm;
using TodoApp.Application.Features.OrderHandle.Command.Create;
using TodoApp.Application.Features.OrderHandle.Command.Ship;
using TodoApp.Application.Features.OrderHandle.Query.GetAll;
using TodoApp.Application.Features.OrderHandle.Query.GetById;

namespace TodoApp.WebAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả đơn hàng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] string? status, [FromQuery] int? userId)
        {
            var query = new GetAllOrdersQuery 
            { 
                Status = status, 
                UserId = userId 
            };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return StatusCode(500, new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Lấy danh sách đơn hàng thành công", data = result.Data });
        }

        /// <summary>
        /// Tạo đơn hàng mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.Common.ErrorType.Validation => BadRequest(new { message = result.ErrorMessage, errors = result.Errors }),
                    Application.Common.ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                    _ => StatusCode(500, new { message = result.ErrorMessage })
                };
            }

            return CreatedAtAction(
                nameof(GetOrderById),
                new { id = result.Data!.IdOrder },
                new { message = "Tạo đơn hàng thành công", data = result.Data });
        }

        /// <summary>
        /// Lấy thông tin đơn hàng theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var query = new GetOrderByIdQuery { IdOrder = id };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.Common.ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                    _ => StatusCode(500, new { message = result.ErrorMessage })
                };
            }

            return Ok(new { message = "Lấy thông tin đơn hàng thành công", data = result.Data });
        }

        /// <summary>
        /// Xác nhận đơn hàng
        /// </summary>
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var command = new ConfirmOrderCommand { IdOrder = id };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.Common.ErrorType.Validation => BadRequest(new { message = result.ErrorMessage }),
                    Application.Common.ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                    _ => StatusCode(500, new { message = result.ErrorMessage })
                };
            }

            return Ok(new { message = result.Data });
        }

        /// <summary>
        /// Bắt đầu giao hàng
        /// </summary>
        [HttpPost("{id}/ship")]
        public async Task<IActionResult> ShipOrder(int id, [FromBody] ShipOrderRequest request)
        {
            var command = new ShipOrderCommand 
            { 
                IdOrder = id, 
                TrackingNumber = request.TrackingNumber 
            };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.Common.ErrorType.Validation => BadRequest(new { message = result.ErrorMessage }),
                    Application.Common.ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                    _ => StatusCode(500, new { message = result.ErrorMessage })
                };
            }

            return Ok(new { message = result.Data });
        }

        /// <summary>
        /// Hủy đơn hàng
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderRequest request)
        {
            var command = new CancelOrderCommand 
            { 
                IdOrder = id, 
                Reason = request.Reason 
            };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    Application.Common.ErrorType.Validation => BadRequest(new { message = result.ErrorMessage }),
                    Application.Common.ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                    _ => StatusCode(500, new { message = result.ErrorMessage })
                };
            }

            return Ok(new { message = result.Data });
        }
    }

    // Request models
    public record ShipOrderRequest(string? TrackingNumber);
    public record CancelOrderRequest(string Reason);
}
