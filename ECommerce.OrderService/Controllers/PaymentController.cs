using ECommerce.OrderService.DTOs;
using ECommerce.OrderService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.OrderService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            CreatePaymentDto dto)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                var payment =
                    await _paymentService.CreatePaymentAsync(
                        userId,
                        dto.OrderId);

                var paymentDto = new PaymentDto
                {
                    Id = payment.Id,
                    OrderId = payment.OrderId,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    PaymentMethod = payment.PaymentMethod,
                    RazorpayOrderId = payment.RazorpayOrderId,
                    RazorpayPaymentId = payment.RazorpayPaymentId,
                    PaymentDate = payment.PaymentDate
                };

                return Ok(paymentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(
            int orderId)
        {
            var currentUserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var payment =
                await _paymentService.GetByOrderIdAsync(orderId);

            if (payment == null)
                return NotFound();

            if (payment.Order.UserId != currentUserId)
                return Forbid();

            var paymentDto = new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                RazorpayOrderId = payment.RazorpayOrderId,
                RazorpayPaymentId = payment.RazorpayPaymentId,
                PaymentDate = payment.PaymentDate
            };

            return Ok(paymentDto);
        }

        [HttpPost("cancel/{orderId}")]
        public async Task<IActionResult> CancelPayment(
            int orderId)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                await _paymentService.CancelPaymentAsync(
                    userId,
                    orderId);

                return Ok("Payment cancelled successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment(
            VerifyPaymentDto dto)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                var result =
                    await _paymentService.VerifyPaymentAsync(
                        userId,
                        dto.RazorpayOrderId,
                        dto.RazorpayPaymentId,
                        dto.RazorpaySignature);

                if (!result)
                    return BadRequest(
                        "Payment verification failed.");

                return Ok(
                    "Payment verified successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}