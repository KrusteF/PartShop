using System;

using PartShop.Models;

namespace PartShop.Utility
{
    public static class SD
    {
        public const string DefaultToolImage = "default_tool.jpg";

        public const string AdminUser = "Admin";
        public const string CustomerUser = "Customer";
        public const string ssShoppingCartCount = "ssCartCount";
        public const string ssCouponCode = "ssCouponCode";

        public const string StatusSubmitted = "Submitted";
        public const string StatusInProcess = "Being Prepared";
        public const string StatusReady = "Ready for Pickup";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";

        public const string OrderPlacementStatus = "OrderPlaced.png";
        public const string OrderAcceptedStatus = "InKitchen.png";
        public const string ReadyForPickupStatus = "ReadyForPickup.png";
        public const string CompletedStatus = "completed.png";

        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static double DiscountPrice(Coupon couponFromDb, double OrignalOrderTotal)
        {
            if (couponFromDb == null)
            {
                return OrignalOrderTotal;
            }
            else
            {
                if (couponFromDb.MinimumAmount > OrignalOrderTotal)
                {
                    return OrignalOrderTotal;
                }
                else
                {
                    if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Сума)
                    {
                        return Math.Round(OrignalOrderTotal - couponFromDb.Discount, 2);
                    }
                    else
                    {
                        if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Процент)
                        {
                            return Math.Round(OrignalOrderTotal - (OrignalOrderTotal * couponFromDb.Discount / 100), 2);
                        }
                    }
                }
            }
            return OrignalOrderTotal;
        }

    }
}
