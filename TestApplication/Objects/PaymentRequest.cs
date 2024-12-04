namespace TestApplication.Objects
{
    public class PaymentRequest
    {
        public string Id { get; set; } = string.Empty;
        public double amount { get; set; }
        public string currency { get; set; } = string.Empty;
        public enum Status
        {
            completed,
            pending,
            failed,
        }
        public enum payment_method
        {
            momo,
            card,
            bank_transfer,
        }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string accesstoken { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;

    }

    public class PaymentResponse
    {
        public string PaymentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        // Add other relevant properties related to payment response
    }
    public class PaymentDetails
    {
        public string PaymentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        // Add other relevant payment detail properties
    }
}
