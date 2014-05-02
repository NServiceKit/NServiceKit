using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceKit.DesignPatterns.Model;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>A sample order line.</summary>
	public class SampleOrderLine
		: IHasStringId
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public string Id { get; set; }

        /// <summary>Gets the order URN.</summary>
        ///
        /// <value>The order URN.</value>
		public string OrderUrn
		{
			get
			{
				return CreateUrn(this.UserId, this.OrderId, this.OrderLineId);
			}
		}

        /// <summary>Gets or sets the identifier of the order.</summary>
        ///
        /// <value>The identifier of the order.</value>
		public long OrderId { get; set; }

        /// <summary>Gets or sets the identifier of the order line.</summary>
        ///
        /// <value>The identifier of the order line.</value>
		public long OrderLineId { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        ///
        /// <value>The created date.</value>
		public DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the identifier of the user.</summary>
        ///
        /// <value>The identifier of the user.</value>
		public Guid UserId { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
		public string UserName { get; set; }

        /// <summary>Gets or sets the identifier of the product.</summary>
        ///
        /// <value>The identifier of the product.</value>
		public Guid ProductId { get; set; }

        /// <summary>Gets or sets the mflow URN.</summary>
        ///
        /// <value>The mflow URN.</value>
		public string MflowUrn { get; set; }

        /// <summary>Gets or sets the type of the product.</summary>
        ///
        /// <value>The type of the product.</value>
		public string ProductType { get; set; }

        /// <summary>Gets or sets the description.</summary>
        ///
        /// <value>The description.</value>
		public string Description { get; set; }

        /// <summary>Gets or sets the upc ean.</summary>
        ///
        /// <value>The upc ean.</value>
		public string UpcEan { get; set; }

        /// <summary>Gets or sets the isrc.</summary>
        ///
        /// <value>The isrc.</value>
		public string Isrc { get; set; }

        /// <summary>Gets or sets the identifier of the recommendation user.</summary>
        ///
        /// <value>The identifier of the recommendation user.</value>
		public Guid? RecommendationUserId { get; set; }

        /// <summary>Gets or sets the name of the recommendation user.</summary>
        ///
        /// <value>The name of the recommendation user.</value>
		public string RecommendationUserName { get; set; }

        /// <summary>Gets or sets the name of the supplier key.</summary>
        ///
        /// <value>The name of the supplier key.</value>
		public string SupplierKeyName { get; set; }

        /// <summary>Gets or sets the name of the cost tier key.</summary>
        ///
        /// <value>The name of the cost tier key.</value>
		public string CostTierKeyName { get; set; }

        /// <summary>Gets or sets the name of the price tier key.</summary>
        ///
        /// <value>The name of the price tier key.</value>
		public string PriceTierKeyName { get; set; }

        /// <summary>Gets or sets the vat rate.</summary>
        ///
        /// <value>The vat rate.</value>
		public decimal VatRate { get; set; }

        /// <summary>Gets or sets the product price increment vat.</summary>
        ///
        /// <value>The product price increment vat.</value>
		public int ProductPriceIncVat { get; set; }

        /// <summary>Gets or sets the quantity.</summary>
        ///
        /// <value>The quantity.</value>
		public int Quantity { get; set; }

        /// <summary>Gets or sets the transaction value ex vat.</summary>
        ///
        /// <value>The transaction value ex vat.</value>
		public decimal TransactionValueExVat { get; set; }

        /// <summary>Gets or sets the transaction value increment vat.</summary>
        ///
        /// <value>The transaction value increment vat.</value>
		public decimal TransactionValueIncVat { get; set; }

        /// <summary>Gets or sets the recommendation discount rate.</summary>
        ///
        /// <value>The recommendation discount rate.</value>
		public decimal RecommendationDiscountRate { get; set; }

        /// <summary>Gets or sets the distribution discount rate.</summary>
        ///
        /// <value>The distribution discount rate.</value>
		public decimal DistributionDiscountRate { get; set; }

        /// <summary>Gets or sets the recommendation discount accrued ex vat.</summary>
        ///
        /// <value>The recommendation discount accrued ex vat.</value>
		public decimal RecommendationDiscountAccruedExVat { get; set; }

        /// <summary>Gets or sets the distribution discount accrued ex vat.</summary>
        ///
        /// <value>The distribution discount accrued ex vat.</value>
		public decimal DistributionDiscountAccruedExVat { get; set; }

        /// <summary>Gets or sets the promo mix.</summary>
        ///
        /// <value>The promo mix.</value>
		public decimal PromoMix { get; set; }

        /// <summary>Gets or sets the discount mix.</summary>
        ///
        /// <value>The discount mix.</value>
		public decimal DiscountMix { get; set; }

        /// <summary>Gets or sets the cash mix.</summary>
        ///
        /// <value>The cash mix.</value>
		public decimal CashMix { get; set; }

        /// <summary>Gets or sets the promo mix value ex vat.</summary>
        ///
        /// <value>The promo mix value ex vat.</value>
		public decimal PromoMixValueExVat { get; set; }

        /// <summary>Gets or sets the discount mix value ex vat.</summary>
        ///
        /// <value>The discount mix value ex vat.</value>
		public decimal DiscountMixValueExVat { get; set; }

        /// <summary>Gets or sets the cash mix value increment vat.</summary>
        ///
        /// <value>The cash mix value increment vat.</value>
		public decimal CashMixValueIncVat { get; set; }

        /// <summary>Gets or sets the content URN.</summary>
        ///
        /// <value>The content URN.</value>
		public string ContentUrn
		{
			get { return this.MflowUrn; }
			set { this.MflowUrn = value; }
		}

        /// <summary>Gets or sets the track URN.</summary>
        ///
        /// <value>The track URN.</value>
		public string TrackUrn
		{
			get;
			set;
		}

        /// <summary>Gets or sets the title.</summary>
        ///
        /// <value>The title.</value>
		public string Title
		{
			get;
			set;
		}

        /// <summary>Gets or sets the artist URN.</summary>
        ///
        /// <value>The artist URN.</value>
		public string ArtistUrn
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name of the artist.</summary>
        ///
        /// <value>The name of the artist.</value>
		public string ArtistName
		{
			get;
			set;
		}

        /// <summary>Gets or sets the album URN.</summary>
        ///
        /// <value>The album URN.</value>
		public string AlbumUrn
		{
			get;
			set;
		}

        /// <summary>Gets or sets the name of the album.</summary>
        ///
        /// <value>The name of the album.</value>
		public string AlbumName
		{
			get;
			set;
		}

        /// <summary>Creates an URN.</summary>
        ///
        /// <param name="userId">     Identifier for the user.</param>
        /// <param name="orderId">    Identifier for the order.</param>
        /// <param name="orderLineId">Identifier for the order line.</param>
        ///
        /// <returns>The new URN.</returns>
		public static string CreateUrn(Guid userId, long orderId, long orderLineId)
		{
			return string.Format("urn:orderline:{0}/{1}/{2}",
								 userId.ToString("N"), orderId, orderLineId);
		}

        /// <summary>Creates a new SampleOrderLine.</summary>
        ///
        /// <param name="userId">Identifier for the user.</param>
        ///
        /// <returns>A SampleOrderLine.</returns>
		public static SampleOrderLine Create(Guid userId)
		{
			return Create(userId, 1, 1);
		}

        /// <summary>Creates a new SampleOrderLine.</summary>
        ///
        /// <param name="userId">     Identifier for the user.</param>
        /// <param name="orderId">    Identifier for the order.</param>
        /// <param name="orderLineId">Identifier for the order line.</param>
        ///
        /// <returns>A SampleOrderLine.</returns>
		public static SampleOrderLine Create(Guid userId, int orderId, int orderLineId)
		{
			return new SampleOrderLine {
				Id = CreateUrn(userId, orderId, orderLineId),
				CreatedDate = DateTime.Now,
				OrderId = orderId,
				OrderLineId = orderLineId,
				AlbumName = "AlbumName",
				CashMixValueIncVat = 0.79m / 1.15m,
				TransactionValueExVat = 0.79m,
				ContentUrn = "urn:content:" + Guid.NewGuid().ToString("N"),
			};
		}

	}
}