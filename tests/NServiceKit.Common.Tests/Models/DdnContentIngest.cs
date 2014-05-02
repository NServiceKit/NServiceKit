using System;
using System.Collections.Generic;
using System.Linq;
using NServiceKit.Common.Extensions;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests.Models
{
    /// <summary>Interface for external.</summary>
	public interface IExternal
	{
        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		string ExternalUrn { get; set; }
	}

    /// <summary>Interface for external deletable.</summary>
	public interface IExternalDeletable : IExternal
	{
        /// <summary>Gets or sets a value indicating whether the delete.</summary>
        ///
        /// <value>true if delete, false if not.</value>
		bool Delete { get; set; }
	}

    /// <summary>Interface for content.</summary>
	public interface IContent : IExternal
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		Guid Id { get; set; }

        /// <summary>Gets or sets the URN.</summary>
        ///
        /// <value>The URN.</value>
		string Urn { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        ///
        /// <value>The created date.</value>
		DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the modified date.</summary>
        ///
        /// <value>The modified date.</value>
		DateTime ModifiedDate { get; set; }
	}

    /// <summary>Interface for content deletable.</summary>
	public interface IContentDeletable : IContent
	{
        /// <summary>Gets or sets the deleted date.</summary>
        ///
        /// <value>The deleted date.</value>
		DateTime? DeletedDate { get; set; }
	}

    /// <summary>List of merges.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public class MergeList<T>
	{
        /// <summary>Gets or sets a value indicating whether the partial.</summary>
        ///
        /// <value>true if partial, false if not.</value>
		public bool Partial { get; set; }

        /// <summary>Gets or sets the items.</summary>
        ///
        /// <value>The items.</value>
		public List<T> Items { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.Models.MergeList&lt;T&gt; class.</summary>
		public MergeList()
		{
		}

        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.Models.MergeList&lt;T&gt; class.</summary>
        ///
        /// <param name="items">The items.</param>
		public MergeList(IEnumerable<T> items)
		{
			this.Items = new List<T>(items);
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1} {2}s", this.Partial ? "Partial" : "Full", this.Items.NullableCount(), typeof(T).Name);
		}
	}

    /// <summary>A cost point.</summary>
	public class CostPoint
	{
        /// <summary>Gets or sets the campaign.</summary>
        ///
        /// <value>The campaign.</value>
		public string Campaign { get; set; }

        /// <summary>Gets or sets the start date.</summary>
        ///
        /// <value>The start date.</value>
		public DateTime StartDate { get; set; }

        /// <summary>Gets or sets the cost code.</summary>
        ///
        /// <value>The cost code.</value>
		public string CostCode { get; set; }
	}

    /// <summary>Values that represent ExplicitType.</summary>
	public enum ExplicitType
	{
        /// <summary>An enum constant representing the unknown option.</summary>
		Unknown,

        /// <summary>An enum constant representing the not explicit option.</summary>
		NotExplicit,

        /// <summary>An enum constant representing the explicit option.</summary>
		Explicit,

        /// <summary>An enum constant representing the cleaned option.</summary>
		Cleaned
	}

    /// <summary>Values that represent ReleaseType.</summary>
	public enum ReleaseType
	{
        /// <summary>An enum constant representing the single option.</summary>
		Single,

        /// <summary>An enum constant representing the album option.</summary>
		Album,

        /// <summary>An enum constant representing the ep option.</summary>
		Ep,

        /// <summary>An enum constant representing the boxed set option.</summary>
		BoxedSet
	}

    /// <summary>A batch.</summary>
	public class Batch
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public Guid Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the full pathname of the file.</summary>
        ///
        /// <value>The full pathname of the file.</value>
		public string Path { get; set; }

        /// <summary>Gets or sets the name of the supplier key.</summary>
        ///
        /// <value>The name of the supplier key.</value>
		public string SupplierKeyName { get; set; }

        /// <summary>Gets or sets the name of the supplier.</summary>
        ///
        /// <value>The name of the supplier.</value>
		public string SupplierName { get; set; }

        /// <summary>Gets or sets the name of the delivery key.</summary>
        ///
        /// <value>The name of the delivery key.</value>
		public string DeliveryKeyName { get; set; }

        /// <summary>Gets or sets the sequence number.</summary>
        ///
        /// <value>The sequence number.</value>
		public long SequenceNumber { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1}", this.SupplierKeyName, this.Name);
		}
	}

    /// <summary>A participant.</summary>
	public class Participant
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the role.</summary>
        ///
        /// <value>The role.</value>
		public string Role { get; set; }

        /// <summary>Gets or sets the external reference.</summary>
        ///
        /// <value>The external reference.</value>
		public string ExternalRef { get; set; }

        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		public string ExternalUrn { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1}", this.Name, this.Role);
		}
	}

    /// <summary>An artist update.</summary>
	public class ArtistUpdate : IExternal
	{
        /// <summary>Gets or sets the external reference.</summary>
        ///
        /// <value>The external reference.</value>
		public string ExternalRef { get; set; }

        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		public string ExternalUrn { get; set; }

        /// <summary>Gets or sets the name of the supplier key.</summary>
        ///
        /// <value>The name of the supplier key.</value>
		public string SupplierKeyName { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the birth date.</summary>
        ///
        /// <value>The birth date.</value>
		public DateTime? BirthDate { get; set; }

        /// <summary>Gets or sets the birth place.</summary>
        ///
        /// <value>The birth place.</value>
		public string BirthPlace { get; set; }

        /// <summary>Gets or sets the death date.</summary>
        ///
        /// <value>The death date.</value>
		public DateTime? DeathDate { get; set; }

        /// <summary>Gets or sets the death place.</summary>
        ///
        /// <value>The death place.</value>
		public string DeathPlace { get; set; }

        /// <summary>Gets or sets the decades active.</summary>
        ///
        /// <value>The decades active.</value>
		public string DecadesActive { get; set; }

        /// <summary>Gets or sets the biography.</summary>
        ///
        /// <value>The biography.</value>
		public string Biography { get; set; }

        /// <summary>Gets or sets the biography author.</summary>
        ///
        /// <value>The biography author.</value>
		public string BiographyAuthor { get; set; }

        /// <summary>Gets or sets a list of identifiers of the assets.</summary>
        ///
        /// <value>A list of identifiers of the assets.</value>
		public List<AssetId> AssetIds { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1}", this.Name, this.ExternalRef);
		}
	}

    /// <summary>An asset update.</summary>
	public class AssetUpdate : IExternal
	{
        /// <summary>Gets or sets the external reference.</summary>
        ///
        /// <value>The external reference.</value>
		public string ExternalRef { get; set; }

        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		public string ExternalUrn { get; set; }

        /// <summary>Gets or sets the name of the supplier key.</summary>
        ///
        /// <value>The name of the supplier key.</value>
		public string SupplierKeyName { get; set; }

        /// <summary>Gets or sets the type of the asset.</summary>
        ///
        /// <value>The type of the asset.</value>
		public AssetType AssetType { get; set; }

        /// <summary>Gets or sets the external owner URN.</summary>
        ///
        /// <value>The external owner URN.</value>
		public string ExternalOwnerUrn { get; set; }

        /// <summary>Gets or sets the filename of the file.</summary>
        ///
        /// <value>The name of the file.</value>
		public string FileName { get; set; }

        /// <summary>Gets or sets the file extension.</summary>
        ///
        /// <value>The file extension.</value>
		public string FileExtension { get; set; }

        /// <summary>Gets or sets the file size bytes.</summary>
        ///
        /// <value>The file size bytes.</value>
		public long FileSizeBytes { get; set; }

        /// <summary>Gets or sets the master sha 256 checksum.</summary>
        ///
        /// <value>The master sha 256 checksum.</value>
		public string MasterSha256Checksum { get; set; }

        /// <summary>Gets or sets the md 5 checksum.</summary>
        ///
        /// <value>The md 5 checksum.</value>
		public string Md5Checksum { get; set; }

        /// <summary>Gets or sets the duration milliseconds.</summary>
        ///
        /// <value>The duration milliseconds.</value>
		public int? DurationMs { get; set; }

        /// <summary>Gets or sets the bit rate kbps.</summary>
        ///
        /// <value>The bit rate kbps.</value>
		public int? BitRateKbps { get; set; }

        /// <summary>Gets or sets the width.</summary>
        ///
        /// <value>The width.</value>
		public int? Width { get; set; }

        /// <summary>Gets or sets the height.</summary>
        ///
        /// <value>The height.</value>
		public int? Height { get; set; }

        /// <summary>Gets or sets the full pathname of the transcoded asset file.</summary>
        ///
        /// <value>The full pathname of the transcoded asset file.</value>
		public string TranscodedAssetPath { get; set; }

        /// <summary>Builds external reference.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <returns>A string.</returns>
		public string BuildExternalRef()
		{
			switch (this.AssetType)
			{
				case AssetType.MasterCoverArt:
				case AssetType.MasterArtistArt:
				case AssetType.MasterLabelArt:
					{
						return String.Format("{0}/{1}", this.AssetType, this.MasterSha256Checksum).ToLower();
					}
				case AssetType.TrackProduct:
				case AssetType.TrackPreview:
					{
						var externalOwnerRef = Urn.Parse(this.ExternalOwnerUrn).IdValue;
						return String.Format("{0}/{1}/{2}/{3}/{4}/{5}", this.AssetType, this.MasterSha256Checksum, externalOwnerRef, this.FileExtension.ToLower(), this.BitRateKbps ?? 0, this.DurationMs ?? 0).ToLower();
					}
				default:
					{
						var message = String.Format("AssetType not supported: {0}-{1}-{2}", AssetType, FileExtension.ToLower(), MasterSha256Checksum);
						throw new Exception(message);
					}
			}
		}

        /// <summary>Creates asset identifier.</summary>
        ///
        /// <returns>The new asset identifier.</returns>
		public AssetId CreateAssetId()
		{
			return new AssetId { AssetType = this.AssetType, ExternalUrn = this.ExternalUrn };
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return this.ExternalRef;
		}
	}

    /// <summary>A label update.</summary>
	public class LabelUpdate : IExternal
	{
        /// <summary>Gets or sets the external reference.</summary>
        ///
        /// <value>The external reference.</value>
		public string ExternalRef { get; set; }

        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		public string ExternalUrn { get; set; }

        /// <summary>Gets or sets the name of the supplier key.</summary>
        ///
        /// <value>The name of the supplier key.</value>
		public string SupplierKeyName { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the established date.</summary>
        ///
        /// <value>The established date.</value>
		public DateTime? EstablishedDate { get; set; }

        /// <summary>Gets or sets the biography.</summary>
        ///
        /// <value>The biography.</value>
		public string Biography { get; set; }

        /// <summary>Gets or sets the biography author.</summary>
        ///
        /// <value>The biography author.</value>
		public string BiographyAuthor { get; set; }

        /// <summary>Gets or sets a list of identifiers of the assets.</summary>
        ///
        /// <value>A list of identifiers of the assets.</value>
		public List<AssetId> AssetIds { get; set; }

        /// <summary>Gets image asset identifiers.</summary>
        ///
        /// <returns>The image asset identifiers.</returns>
		public List<AssetId> GetImageAssetIds()
		{
			return AssetIds.Where(x => x.AssetType.IsImage()).ToList();
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1}", this.Name, this.ExternalRef);
		}
	}

    /// <summary>A product update.</summary>
	public class ProductUpdate
	{
        /// <summary>Gets or sets a value indicating whether the delete.</summary>
        ///
        /// <value>true if delete, false if not.</value>
		public bool Delete { get; set; }

        /// <summary>Gets or sets the territory code.</summary>
        ///
        /// <value>The territory code.</value>
		public string TerritoryCode { get; set; }

        /// <summary>Gets or sets the copyright.</summary>
        ///
        /// <value>The copyright.</value>
		public string Copyright { get; set; }

        /// <summary>Gets or sets the report reference.</summary>
        ///
        /// <value>The report reference.</value>
		public string ReportRef { get; set; }

        /// <summary>Gets or sets a value indicating whether we allow download.</summary>
        ///
        /// <value>true if allow download, false if not.</value>
		public bool AllowDownload { get; set; }

        /// <summary>Gets or sets a value indicating whether we allow streaming.</summary>
        ///
        /// <value>true if allow streaming, false if not.</value>
		public bool AllowStreaming { get; set; }

        /// <summary>Gets or sets a value indicating whether we allow subscription.</summary>
        ///
        /// <value>true if allow subscription, false if not.</value>
		public bool AllowSubscription { get; set; }

        /// <summary>Gets or sets a value indicating whether the collection only.</summary>
        ///
        /// <value>true if collection only, false if not.</value>
		public bool CollectionOnly { get; set; }

        /// <summary>Gets or sets the download start date.</summary>
        ///
        /// <value>The download start date.</value>
		public DateTime? DownloadStartDate { get; set; }

        /// <summary>Gets or sets the download end date.</summary>
        ///
        /// <value>The download end date.</value>
		public DateTime? DownloadEndDate { get; set; }

        /// <summary>Gets or sets the cost points.</summary>
        ///
        /// <value>The cost points.</value>
		public List<CostPoint> CostPoints { get; set; }

        /// <summary>Gets or sets the streaming start date.</summary>
        ///
        /// <value>The streaming start date.</value>
		public DateTime? StreamingStartDate { get; set; }

        /// <summary>Gets or sets the streaming end date.</summary>
        ///
        /// <value>The streaming end date.</value>
		public DateTime? StreamingEndDate { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return this.TerritoryCode;
		}
	}

    /// <summary>A release change set.</summary>
	public class ReleaseChangeSet
	{
        /// <summary>Gets or sets the identifier of the batch.</summary>
        ///
        /// <value>The identifier of the batch.</value>
		public Guid BatchId { get; set; }

        /// <summary>Gets or sets the name of the batch.</summary>
        ///
        /// <value>The name of the batch.</value>
		public string BatchName { get; set; }

        /// <summary>Gets or sets the batch sequence.</summary>
        ///
        /// <value>The batch sequence.</value>
		public long BatchSequence { get; set; }

        /// <summary>Gets or sets the full pathname of the batch file.</summary>
        ///
        /// <value>The full pathname of the batch file.</value>
		public string BatchPath { get; set; }

        /// <summary>Gets or sets a value indicating whether the partial.</summary>
        ///
        /// <value>true if partial, false if not.</value>
		public bool Partial { get; set; }

        /// <summary>Gets or sets the release.</summary>
        ///
        /// <value>The release.</value>
		public ReleaseUpdate Release { get; set; }

        /// <summary>Gets or sets the labels.</summary>
        ///
        /// <value>The labels.</value>
		public List<LabelUpdate> Labels { get; set; }

        /// <summary>Gets or sets the artists.</summary>
        ///
        /// <value>The artists.</value>
		public List<ArtistUpdate> Artists { get; set; }

        /// <summary>Gets or sets the tracks.</summary>
        ///
        /// <value>The tracks.</value>
		public List<TrackUpdate> Tracks { get; set; }

        /// <summary>Gets or sets the assets.</summary>
        ///
        /// <value>The assets.</value>
		public List<AssetUpdate> Assets { get; set; }

        /// <summary>Updates the batch information described by batch.</summary>
        ///
        /// <param name="batch">The batch.</param>
		public void UpdateBatchInfo(Batch batch)
		{
			this.BatchId = batch.Id;
			this.BatchName = batch.Name;
			this.BatchSequence = batch.SequenceNumber;
			this.BatchPath = batch.Path;
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1} {2}", this.Release.ExternalUrn, this.BatchName, this.Partial ? "Partial" : "Full");
		}
	}

    /// <summary>A release change set serializer.</summary>
	public static class ReleaseChangeSetSerializer
	{
        /// <summary>true this object to the given stream.</summary>
        ///
        /// <param name="changeSet">Set the change belongs to.</param>
        ///
        /// <returns>A string.</returns>
		public static string Serialize(ReleaseChangeSet changeSet)
		{
			return TypeSerializer.SerializeToString(changeSet);
		}

        /// <summary>true this object to the given stream.</summary>
        ///
        /// <param name="changeSetString">The change set string.</param>
        ///
        /// <returns>A ReleaseChangeSet.</returns>
		public static ReleaseChangeSet Deserialize(string changeSetString)
		{
			return TypeSerializer.DeserializeFromString<ReleaseChangeSet>(changeSetString);
		}
	}

    /// <summary>A track update.</summary>
	public class TrackUpdate : IExternalDeletable
	{
        /// <summary>Gets or sets a value indicating whether the delete.</summary>
        ///
        /// <value>true if delete, false if not.</value>
		public bool Delete { get; set; }

        /// <summary>Gets or sets the external reference.</summary>
        ///
        /// <value>The external reference.</value>
		public string ExternalRef { get; set; }

        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		public string ExternalUrn { get; set; }

        /// <summary>Gets or sets the name of the supplier key.</summary>
        ///
        /// <value>The name of the supplier key.</value>
		public string SupplierKeyName { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the name ex version.</summary>
        ///
        /// <value>The name ex version.</value>
		public string NameExVersion { get; set; }

        /// <summary>Gets or sets the name version.</summary>
        ///
        /// <value>The name version.</value>
		public string NameVersion { get; set; }

        /// <summary>Gets or sets the label text.</summary>
        ///
        /// <value>The label text.</value>
		public string LabelText { get; set; }

        /// <summary>Gets or sets the artist text.</summary>
        ///
        /// <value>The artist text.</value>
		public string ArtistText { get; set; }

        /// <summary>Gets or sets the release text.</summary>
        ///
        /// <value>The release text.</value>
		public string ReleaseText { get; set; }

        /// <summary>Gets or sets the isrc.</summary>
        ///
        /// <value>The isrc.</value>
		public string Isrc { get; set; }

        /// <summary>Gets or sets the identifier of the global release.</summary>
        ///
        /// <value>The identifier of the global release.</value>
		public string GlobalReleaseId { get; set; }

        /// <summary>Gets or sets the set number.</summary>
        ///
        /// <value>The set number.</value>
		public int? SetNumber { get; set; }

        /// <summary>Gets or sets the disc number.</summary>
        ///
        /// <value>The disc number.</value>
		public int? DiscNumber { get; set; }

        /// <summary>Gets or sets the track number.</summary>
        ///
        /// <value>The track number.</value>
		public int? TrackNumber { get; set; }

        /// <summary>Gets or sets the sequence number.</summary>
        ///
        /// <value>The sequence number.</value>
		public int? SequenceNumber { get; set; }

        /// <summary>Gets or sets the duration milliseconds.</summary>
        ///
        /// <value>The duration milliseconds.</value>
		public int? DurationMs { get; set; }

        /// <summary>Gets or sets the type of the explicit.</summary>
        ///
        /// <value>The type of the explicit.</value>
		public ExplicitType ExplicitType { get; set; }

        /// <summary>Gets or sets the rights holder.</summary>
        ///
        /// <value>The rights holder.</value>
		public string RightsHolder { get; set; }

        /// <summary>Gets or sets the copyright.</summary>
        ///
        /// <value>The copyright.</value>
		public string Copyright { get; set; }

        /// <summary>Gets or sets the publishers.</summary>
        ///
        /// <value>The publishers.</value>
		public List<string> Publishers { get; set; }

        /// <summary>Gets or sets the genres.</summary>
        ///
        /// <value>The genres.</value>
		public List<string> Genres { get; set; }

        /// <summary>Gets or sets the sub genres.</summary>
        ///
        /// <value>The sub genres.</value>
		public List<string> SubGenres { get; set; }

        /// <summary>Gets or sets the review.</summary>
        ///
        /// <value>The review.</value>
		public string Review { get; set; }

        /// <summary>Gets or sets the review author.</summary>
        ///
        /// <value>The review author.</value>
		public string ReviewAuthor { get; set; }

        /// <summary>Gets or sets the lyrics.</summary>
        ///
        /// <value>The lyrics.</value>
		public string Lyrics { get; set; }

        /// <summary>Gets or sets the participants.</summary>
        ///
        /// <value>The participants.</value>
		public List<Participant> Participants { get; set; }

        /// <summary>Gets or sets the external label URN.</summary>
        ///
        /// <value>The external label URN.</value>
		public string ExternalLabelUrn { get; set; }

        /// <summary>Gets or sets the external release URN.</summary>
        ///
        /// <value>The external release URN.</value>
		public string ExternalReleaseUrn { get; set; }

        /// <summary>Gets or sets the external artist urns.</summary>
        ///
        /// <value>The external artist urns.</value>
		public MergeList<string> ExternalArtistUrns { get; set; }

        /// <summary>Gets or sets the products.</summary>
        ///
        /// <value>The products.</value>
		public MergeList<ProductUpdate> Products { get; set; }

        /// <summary>Gets or sets a list of identifiers of the assets.</summary>
        ///
        /// <value>A list of identifiers of the assets.</value>
		public List<AssetId> AssetIds { get; set; }

        /// <summary>Adds an asset identifiers.</summary>
        ///
        /// <param name="assetIds">List of identifiers for the assets.</param>
		public void AddAssetIds(IEnumerable<AssetId> assetIds)
		{
			if (assetIds == null || !assetIds.Any())
			{
				return;
			}

			if (this.AssetIds == null)
			{
				this.AssetIds = new List<AssetId>();
			}

			this.AssetIds.AddRange(assetIds);
		}

        /// <summary>Gets image asset identifiers.</summary>
        ///
        /// <returns>The image asset identifiers.</returns>
		public List<AssetId> GetImageAssetIds()
		{
			return this.AssetIds.SafeWhere(x => x.AssetType.IsImage()).ToList();
		}

        /// <summary>Gets audio asset identifiers.</summary>
        ///
        /// <returns>The audio asset identifiers.</returns>
		public List<AssetId> GetAudioAssetIds()
		{
			return this.AssetIds.SafeWhere(x => x.AssetType.IsAudio()).ToList();
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1}", this.Name, this.ExternalRef);
		}
	}

    /// <summary>A release update.</summary>
	public class ReleaseUpdate : IExternalDeletable
	{
        /// <summary>The world territory code.</summary>
		public const string WorldTerritoryCode = "ZZ";

        /// <summary>Gets or sets a value indicating whether the delete.</summary>
        ///
        /// <value>true if delete, false if not.</value>
		public bool Delete { get; set; }

        /// <summary>Gets or sets the external reference.</summary>
        ///
        /// <value>The external reference.</value>
		public string ExternalRef { get; set; }

        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		public string ExternalUrn { get; set; }

        /// <summary>Gets or sets the name of the supplier key.</summary>
        ///
        /// <value>The name of the supplier key.</value>
		public string SupplierKeyName { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the name ex version.</summary>
        ///
        /// <value>The name ex version.</value>
		public string NameExVersion { get; set; }

        /// <summary>Gets or sets the name version.</summary>
        ///
        /// <value>The name version.</value>
		public string NameVersion { get; set; }

        /// <summary>Gets or sets the type of the release.</summary>
        ///
        /// <value>The type of the release.</value>
		public ReleaseType ReleaseType { get; set; }

        /// <summary>Gets or sets the label text.</summary>
        ///
        /// <value>The label text.</value>
		public string LabelText { get; set; }

        /// <summary>Gets or sets the artist text.</summary>
        ///
        /// <value>The artist text.</value>
		public string ArtistText { get; set; }

        /// <summary>Gets or sets the upc ean.</summary>
        ///
        /// <value>The upc ean.</value>
		public string UpcEan { get; set; }

        /// <summary>Gets or sets the identifier of the global release.</summary>
        ///
        /// <value>The identifier of the global release.</value>
		public string GlobalReleaseId { get; set; }

        /// <summary>Gets or sets the catalogue number.</summary>
        ///
        /// <value>The catalogue number.</value>
		public string CatalogueNumber { get; set; }

        /// <summary>Gets or sets the number of sets.</summary>
        ///
        /// <value>The number of sets.</value>
		public int? SetCount { get; set; }

        /// <summary>Gets or sets the number of discs.</summary>
        ///
        /// <value>The number of discs.</value>
		public int? DiscCount { get; set; }

        /// <summary>Gets or sets the number of tracks.</summary>
        ///
        /// <value>The number of tracks.</value>
		public int? TrackCount { get; set; }

        /// <summary>Gets or sets the duration milliseconds.</summary>
        ///
        /// <value>The duration milliseconds.</value>
		public int? DurationMs { get; set; }

        /// <summary>Gets or sets a value indicating whether the continuous mix.</summary>
        ///
        /// <value>true if continuous mix, false if not.</value>
		public bool ContinuousMix { get; set; }

        /// <summary>Gets or sets the type of the explicit.</summary>
        ///
        /// <value>The type of the explicit.</value>
		public ExplicitType ExplicitType { get; set; }

        /// <summary>Gets or sets the release date.</summary>
        ///
        /// <value>The release date.</value>
		public DateTime? ReleaseDate { get; set; }

        /// <summary>Gets or sets the rights holder.</summary>
        ///
        /// <value>The rights holder.</value>
		public string RightsHolder { get; set; }

        /// <summary>Gets or sets the copyright.</summary>
        ///
        /// <value>The copyright.</value>
		public string Copyright { get; set; }

        /// <summary>Gets or sets the genres.</summary>
        ///
        /// <value>The genres.</value>
		public List<string> Genres { get; set; }

        /// <summary>Gets or sets the sub genres.</summary>
        ///
        /// <value>The sub genres.</value>
		public List<string> SubGenres { get; set; }

        /// <summary>Gets or sets the review.</summary>
        ///
        /// <value>The review.</value>
		public string Review { get; set; }

        /// <summary>Gets or sets the review author.</summary>
        ///
        /// <value>The review author.</value>
		public string ReviewAuthor { get; set; }

        /// <summary>Gets or sets the participants.</summary>
        ///
        /// <value>The participants.</value>
		public List<Participant> Participants { get; set; }

        /// <summary>Gets or sets the external label URN.</summary>
        ///
        /// <value>The external label URN.</value>
		public string ExternalLabelUrn { get; set; }

        /// <summary>Gets or sets the external artist urns.</summary>
        ///
        /// <value>The external artist urns.</value>
		public MergeList<string> ExternalArtistUrns { get; set; }

        /// <summary>Gets or sets the external track urns.</summary>
        ///
        /// <value>The external track urns.</value>
		public MergeList<string> ExternalTrackUrns { get; set; }

        /// <summary>Gets or sets the products.</summary>
        ///
        /// <value>The products.</value>
		public MergeList<ProductUpdate> Products { get; set; }

        /// <summary>Gets or sets a list of identifiers of the assets.</summary>
        ///
        /// <value>A list of identifiers of the assets.</value>
		public List<AssetId> AssetIds { get; set; }

        /// <summary>Adds an asset identifiers.</summary>
        ///
        /// <param name="assetIds">List of identifiers for the assets.</param>
		public void AddAssetIds(IEnumerable<AssetId> assetIds)
		{
			if (assetIds == null || !assetIds.Any())
			{
				return;
			}

			if (this.AssetIds == null)
			{
				this.AssetIds = new List<AssetId>();
			}

			this.AssetIds.AddRange(assetIds);
		}

        /// <summary>Gets image asset identifiers.</summary>
        ///
        /// <returns>The image asset identifiers.</returns>
		public List<AssetId> GetImageAssetIds()
		{
			return this.AssetIds.SafeWhere(x => x.AssetType.IsImage()).ToList();
		}

        /// <summary>Gets audio asset identifiers.</summary>
        ///
        /// <returns>The audio asset identifiers.</returns>
		public List<AssetId> GetAudioAssetIds()
		{
			return this.AssetIds.SafeWhere(x => x.AssetType.IsAudio()).ToList();
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1}", this.Name, this.ExternalRef);
		}
	}

    /// <summary>An asset identifier.</summary>
	public class AssetId
	{
        /// <summary>Gets or sets the type of the asset.</summary>
        ///
        /// <value>The type of the asset.</value>
		public AssetType AssetType { get; set; }

        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		public string ExternalUrn { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return this.ExternalUrn;
		}
	}

    /// <summary>An artist.</summary>
	public class Artist : IContent
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public Guid Id { get; set; }

        /// <summary>Gets or sets the URN.</summary>
        ///
        /// <value>The URN.</value>
		public string Urn { get; set; }

        /// <summary>Gets or sets the external reference.</summary>
        ///
        /// <value>The external reference.</value>
		public string ExternalRef { get; set; }

        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		public string ExternalUrn { get; set; }

        /// <summary>Gets or sets the name of the supplier key.</summary>
        ///
        /// <value>The name of the supplier key.</value>
		public string SupplierKeyName { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the birth date.</summary>
        ///
        /// <value>The birth date.</value>
		public DateTime? BirthDate { get; set; }

        /// <summary>Gets or sets the birth place.</summary>
        ///
        /// <value>The birth place.</value>
		public string BirthPlace { get; set; }

        /// <summary>Gets or sets the death date.</summary>
        ///
        /// <value>The death date.</value>
		public DateTime? DeathDate { get; set; }

        /// <summary>Gets or sets the death place.</summary>
        ///
        /// <value>The death place.</value>
		public string DeathPlace { get; set; }

        /// <summary>Gets or sets the decades active.</summary>
        ///
        /// <value>The decades active.</value>
		public string DecadesActive { get; set; }

        /// <summary>Gets or sets the biography.</summary>
        ///
        /// <value>The biography.</value>
		public string Biography { get; set; }

        /// <summary>Gets or sets the biography author.</summary>
        ///
        /// <value>The biography author.</value>
		public string BiographyAuthor { get; set; }

        /// <summary>Gets or sets the assets.</summary>
        ///
        /// <value>The assets.</value>
		public List<Asset> Assets { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        ///
        /// <value>The created date.</value>
		public DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the modified date.</summary>
        ///
        /// <value>The modified date.</value>
		public DateTime ModifiedDate { get; set; }

        /// <summary>Gets image assets.</summary>
        ///
        /// <returns>The image assets.</returns>
		public List<Asset> GetImageAssets()
		{
			return this.Assets.Where(x => x.AssetType.IsImage()).ToList();
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1}", this.Name, this.Id.ToString("N"));
		}
	}

    /// <summary>An asset.</summary>
	public class Asset : IContent
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public Guid Id { get; set; }

        /// <summary>Gets or sets the URN.</summary>
        ///
        /// <value>The URN.</value>
		public string Urn { get; set; }

        /// <summary>Gets or sets the external reference.</summary>
        ///
        /// <value>The external reference.</value>
		public string ExternalRef { get; set; }

        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		public string ExternalUrn { get; set; }

        /// <summary>Gets or sets the name of the supplier key.</summary>
        ///
        /// <value>The name of the supplier key.</value>
		public string SupplierKeyName { get; set; }

        /// <summary>Gets or sets the type of the asset.</summary>
        ///
        /// <value>The type of the asset.</value>
		public AssetType AssetType { get; set; }

        /// <summary>Gets or sets the external owner URN.</summary>
        ///
        /// <value>The external owner URN.</value>
		public string ExternalOwnerUrn { get; set; }

        /// <summary>Gets or sets the filename of the file.</summary>
        ///
        /// <value>The name of the file.</value>
		public string FileName { get; set; }

        /// <summary>Gets or sets the file extension.</summary>
        ///
        /// <value>The file extension.</value>
		public string FileExtension { get; set; }

        /// <summary>Gets or sets the file size bytes.</summary>
        ///
        /// <value>The file size bytes.</value>
		public long FileSizeBytes { get; set; }

        /// <summary>Gets or sets the master sha 256 checksum.</summary>
        ///
        /// <value>The master sha 256 checksum.</value>
		public string MasterSha256Checksum { get; set; }

        /// <summary>Gets or sets the md 5 checksum.</summary>
        ///
        /// <value>The md 5 checksum.</value>
		public string Md5Checksum { get; set; }

        /// <summary>Gets or sets the duration milliseconds.</summary>
        ///
        /// <value>The duration milliseconds.</value>
		public int? DurationMs { get; set; }

        /// <summary>Gets or sets the bit rate kbps.</summary>
        ///
        /// <value>The bit rate kbps.</value>
		public int? BitRateKbps { get; set; }

        /// <summary>Gets or sets the width.</summary>
        ///
        /// <value>The width.</value>
		public int? Width { get; set; }

        /// <summary>Gets or sets the height.</summary>
        ///
        /// <value>The height.</value>
		public int? Height { get; set; }

        /// <summary>Gets or sets the full pathname of the transcoded asset file.</summary>
        ///
        /// <value>The full pathname of the transcoded asset file.</value>
		public string TranscodedAssetPath { get; set; }

        /// <summary>Gets or sets the full pathname of the repository asset file.</summary>
        ///
        /// <value>The full pathname of the repository asset file.</value>
		public string RepositoryAssetPath { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        ///
        /// <value>The created date.</value>
		public DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the modified date.</summary>
        ///
        /// <value>The modified date.</value>
		public DateTime ModifiedDate { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1}", this.ExternalRef, this.Id.ToString("N"));
		}
	}

    /// <summary>A label.</summary>
	public class Label : IContent
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public Guid Id { get; set; }

        /// <summary>Gets or sets the URN.</summary>
        ///
        /// <value>The URN.</value>
		public string Urn { get; set; }

        /// <summary>Gets or sets the external reference.</summary>
        ///
        /// <value>The external reference.</value>
		public string ExternalRef { get; set; }

        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		public string ExternalUrn { get; set; }

        /// <summary>Gets or sets the name of the supplier key.</summary>
        ///
        /// <value>The name of the supplier key.</value>
		public string SupplierKeyName { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the established date.</summary>
        ///
        /// <value>The established date.</value>
		public DateTime? EstablishedDate { get; set; }

        /// <summary>Gets or sets the biography.</summary>
        ///
        /// <value>The biography.</value>
		public string Biography { get; set; }

        /// <summary>Gets or sets the biography author.</summary>
        ///
        /// <value>The biography author.</value>
		public string BiographyAuthor { get; set; }

        /// <summary>Gets or sets the assets.</summary>
        ///
        /// <value>The assets.</value>
		public List<Asset> Assets { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        ///
        /// <value>The created date.</value>
		public DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the modified date.</summary>
        ///
        /// <value>The modified date.</value>
		public DateTime ModifiedDate { get; set; }

        /// <summary>Gets image assets.</summary>
        ///
        /// <returns>The image assets.</returns>
		public List<Asset> GetImageAssets()
		{
			return this.Assets.Where(x => x.AssetType.IsImage()).ToList();
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1}", this.Name, this.Id.ToString("N"));
		}
	}

    /// <summary>A product.</summary>
	public class Product
	{
        /// <summary>Gets or sets the territory code.</summary>
        ///
        /// <value>The territory code.</value>
		public string TerritoryCode { get; set; }

        /// <summary>Gets or sets the copyright.</summary>
        ///
        /// <value>The copyright.</value>
		public string Copyright { get; set; }

        /// <summary>Gets or sets the report reference.</summary>
        ///
        /// <value>The report reference.</value>
		public string ReportRef { get; set; }

        /// <summary>Gets or sets a value indicating whether we allow download.</summary>
        ///
        /// <value>true if allow download, false if not.</value>
		public bool AllowDownload { get; set; }

        /// <summary>Gets or sets a value indicating whether we allow streaming.</summary>
        ///
        /// <value>true if allow streaming, false if not.</value>
		public bool AllowStreaming { get; set; }

        /// <summary>Gets or sets a value indicating whether we allow subscription.</summary>
        ///
        /// <value>true if allow subscription, false if not.</value>
		public bool AllowSubscription { get; set; }

        /// <summary>Gets or sets a value indicating whether the collection only.</summary>
        ///
        /// <value>true if collection only, false if not.</value>
		public bool CollectionOnly { get; set; }

        /// <summary>Gets or sets the download start date.</summary>
        ///
        /// <value>The download start date.</value>
		public DateTime? DownloadStartDate { get; set; }

        /// <summary>Gets or sets the download end date.</summary>
        ///
        /// <value>The download end date.</value>
		public DateTime? DownloadEndDate { get; set; }

        /// <summary>Gets or sets the cost points.</summary>
        ///
        /// <value>The cost points.</value>
		public List<CostPoint> CostPoints { get; set; }

        /// <summary>Gets or sets the streaming start date.</summary>
        ///
        /// <value>The streaming start date.</value>
		public DateTime? StreamingStartDate { get; set; }

        /// <summary>Gets or sets the streaming end date.</summary>
        ///
        /// <value>The streaming end date.</value>
		public DateTime? StreamingEndDate { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return this.TerritoryCode;
		}
	}


    /// <summary>A release.</summary>
	public class Release : IContent
	{
        /// <summary>The world territory code.</summary>
		public const string WorldTerritoryCode = "ZZ";

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public Guid Id { get; set; }

        /// <summary>Gets or sets the URN.</summary>
        ///
        /// <value>The URN.</value>
		public string Urn { get; set; }

        /// <summary>Gets or sets the external reference.</summary>
        ///
        /// <value>The external reference.</value>
		public string ExternalRef { get; set; }

        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		public string ExternalUrn { get; set; }

        /// <summary>Gets or sets the name of the supplier key.</summary>
        ///
        /// <value>The name of the supplier key.</value>
		public string SupplierKeyName { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the name ex version.</summary>
        ///
        /// <value>The name ex version.</value>
		public string NameExVersion { get; set; }

        /// <summary>Gets or sets the name version.</summary>
        ///
        /// <value>The name version.</value>
		public string NameVersion { get; set; }

        /// <summary>Gets or sets the type of the release.</summary>
        ///
        /// <value>The type of the release.</value>
		public ReleaseType ReleaseType { get; set; }

        /// <summary>Gets or sets the label text.</summary>
        ///
        /// <value>The label text.</value>
		public string LabelText { get; set; }

        /// <summary>Gets or sets the artist text.</summary>
        ///
        /// <value>The artist text.</value>
		public string ArtistText { get; set; }

        /// <summary>Gets or sets the upc ean.</summary>
        ///
        /// <value>The upc ean.</value>
		public string UpcEan { get; set; }

        /// <summary>Gets or sets the identifier of the global release.</summary>
        ///
        /// <value>The identifier of the global release.</value>
		public string GlobalReleaseId { get; set; }

        /// <summary>Gets or sets the catalogue number.</summary>
        ///
        /// <value>The catalogue number.</value>
		public string CatalogueNumber { get; set; }

        /// <summary>Gets or sets the number of sets.</summary>
        ///
        /// <value>The number of sets.</value>
		public int SetCount { get; set; }

        /// <summary>Gets or sets the number of discs.</summary>
        ///
        /// <value>The number of discs.</value>
		public int DiscCount { get; set; }

        /// <summary>Gets or sets the number of tracks.</summary>
        ///
        /// <value>The number of tracks.</value>
		public int TrackCount { get; set; }

        /// <summary>Gets or sets the duration milliseconds.</summary>
        ///
        /// <value>The duration milliseconds.</value>
		public int? DurationMs { get; set; }

        /// <summary>Gets or sets a value indicating whether the continuous mix.</summary>
        ///
        /// <value>true if continuous mix, false if not.</value>
		public bool ContinuousMix { get; set; }

        /// <summary>Gets or sets the type of the explicit.</summary>
        ///
        /// <value>The type of the explicit.</value>
		public ExplicitType ExplicitType { get; set; }

        /// <summary>Gets or sets the release date.</summary>
        ///
        /// <value>The release date.</value>
		public DateTime? ReleaseDate { get; set; }

        /// <summary>Gets or sets the rights holder.</summary>
        ///
        /// <value>The rights holder.</value>
		public string RightsHolder { get; set; }

        /// <summary>Gets or sets the copyright.</summary>
        ///
        /// <value>The copyright.</value>
		public string Copyright { get; set; }

        /// <summary>Gets or sets the genres.</summary>
        ///
        /// <value>The genres.</value>
		public List<string> Genres { get; set; }

        /// <summary>Gets or sets the sub genres.</summary>
        ///
        /// <value>The sub genres.</value>
		public List<string> SubGenres { get; set; }

        /// <summary>Gets or sets the review.</summary>
        ///
        /// <value>The review.</value>
		public string Review { get; set; }

        /// <summary>Gets or sets the review author.</summary>
        ///
        /// <value>The review author.</value>
		public string ReviewAuthor { get; set; }

        /// <summary>Gets or sets the participants.</summary>
        ///
        /// <value>The participants.</value>
		public List<Participant> Participants { get; set; }

        /// <summary>Gets or sets the label.</summary>
        ///
        /// <value>The label.</value>
		public Label Label { get; set; }

        /// <summary>Gets or sets the artists.</summary>
        ///
        /// <value>The artists.</value>
		public List<Artist> Artists { get; set; }

        /// <summary>Gets or sets the tracks.</summary>
        ///
        /// <value>The tracks.</value>
		public List<Track> Tracks { get; set; }

        /// <summary>Gets or sets the products.</summary>
        ///
        /// <value>The products.</value>
		public List<Product> Products { get; set; }

        /// <summary>Gets or sets the assets.</summary>
        ///
        /// <value>The assets.</value>
		public List<Asset> Assets { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        ///
        /// <value>The created date.</value>
		public DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the modified date.</summary>
        ///
        /// <value>The modified date.</value>
		public DateTime ModifiedDate { get; set; }

        /// <summary>Gets or sets the deleted date.</summary>
        ///
        /// <value>The deleted date.</value>
		public DateTime? DeletedDate { get; set; }

        /// <summary>Gets primary genre.</summary>
        ///
        /// <returns>The primary genre.</returns>
		public string GetPrimaryGenre()
		{
			return this.Genres.IsNullOrEmpty() ? null : this.Genres[0];
		}

        /// <summary>Gets primary artist.</summary>
        ///
        /// <returns>The primary artist.</returns>
		public Artist GetPrimaryArtist()
		{
			return this.Artists.IsNullOrEmpty() ? null : this.Artists[0];
		}

        /// <summary>Gets image assets.</summary>
        ///
        /// <returns>The image assets.</returns>
		public List<Asset> GetImageAssets()
		{
			return this.Assets.Where(x => x.AssetType.IsImage()).ToList();
		}

        /// <summary>Gets audio assets.</summary>
        ///
        /// <returns>The audio assets.</returns>
		public List<Asset> GetAudioAssets()
		{
			return this.Assets.Where(x => x.AssetType.IsAudio()).ToList();
		}

        /// <summary>Gets active tracks.</summary>
        ///
        /// <returns>The active tracks.</returns>
		public List<Track> GetActiveTracks()
		{
			return this.Tracks.SafeWhere(x => !x.CollectionOrphan).ToList();
		}

        /// <summary>Gets orphan tracks.</summary>
        ///
        /// <returns>The orphan tracks.</returns>
		public List<Track> GetOrphanTracks()
		{
			return this.Tracks.SafeWhere(x => x.CollectionOrphan).ToList();
		}

        /// <summary>Gets a product.</summary>
        ///
        /// <param name="territoryCode">The territory code.</param>
        ///
        /// <returns>The product.</returns>
		public Product GetProduct(string territoryCode)
		{
			if (this.Products.IsNullOrEmpty())
			{
				return null;
			}

			var product = this.Products.FirstOrDefault(x => x.TerritoryCode.EqualsIgnoreCase(territoryCode));
			if (product == null)
			{
				// Default to the world product if exists
				product = this.Products.FirstOrDefault(x => x.TerritoryCode.EqualsIgnoreCase(WorldTerritoryCode));
			}

			return product;
		}

        /// <summary>Gets the description.</summary>
        ///
        /// <returns>The description.</returns>
		public string GetDescription()
		{
			return String.Format("{0} by {1}", this.Name, this.ArtistText);
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1}", this.Name, this.Id.ToString("N"));
		}
	}

    /// <summary>A track.</summary>
	public class Track : IContentDeletable
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public Guid Id { get; set; }

        /// <summary>Gets or sets the URN.</summary>
        ///
        /// <value>The URN.</value>
		public string Urn { get; set; }

        /// <summary>Gets or sets the external reference.</summary>
        ///
        /// <value>The external reference.</value>
		public string ExternalRef { get; set; }

        /// <summary>Gets or sets the external URN.</summary>
        ///
        /// <value>The external URN.</value>
		public string ExternalUrn { get; set; }

        /// <summary>Gets or sets the name of the supplier key.</summary>
        ///
        /// <value>The name of the supplier key.</value>
		public string SupplierKeyName { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>Gets or sets the name ex version.</summary>
        ///
        /// <value>The name ex version.</value>
		public string NameExVersion { get; set; }

        /// <summary>Gets or sets the name version.</summary>
        ///
        /// <value>The name version.</value>
		public string NameVersion { get; set; }

        /// <summary>Gets or sets the label text.</summary>
        ///
        /// <value>The label text.</value>
		public string LabelText { get; set; }

        /// <summary>Gets or sets the artist text.</summary>
        ///
        /// <value>The artist text.</value>
		public string ArtistText { get; set; }

        /// <summary>Gets or sets the release text.</summary>
        ///
        /// <value>The release text.</value>
		public string ReleaseText { get; set; }

        /// <summary>Gets or sets the isrc.</summary>
        ///
        /// <value>The isrc.</value>
		public string Isrc { get; set; }

        /// <summary>Gets or sets the identifier of the global release.</summary>
        ///
        /// <value>The identifier of the global release.</value>
		public string GlobalReleaseId { get; set; }

        /// <summary>Gets or sets a value indicating whether the collection orphan.</summary>
        ///
        /// <value>true if collection orphan, false if not.</value>
		public bool CollectionOrphan { get; set; }

        /// <summary>Gets or sets the set number.</summary>
        ///
        /// <value>The set number.</value>
		public int SetNumber { get; set; }

        /// <summary>Gets or sets the disc number.</summary>
        ///
        /// <value>The disc number.</value>
		public int DiscNumber { get; set; }

        /// <summary>Gets or sets the track number.</summary>
        ///
        /// <value>The track number.</value>
		public int TrackNumber { get; set; }

        /// <summary>Gets or sets the sequence number.</summary>
        ///
        /// <value>The sequence number.</value>
		public int SequenceNumber { get; set; }

        /// <summary>Gets or sets the duration milliseconds.</summary>
        ///
        /// <value>The duration milliseconds.</value>
		public int? DurationMs { get; set; }

        /// <summary>Gets or sets the type of the explicit.</summary>
        ///
        /// <value>The type of the explicit.</value>
		public ExplicitType ExplicitType { get; set; }

        /// <summary>Gets or sets the rights holder.</summary>
        ///
        /// <value>The rights holder.</value>
		public string RightsHolder { get; set; }

        /// <summary>Gets or sets the copyright.</summary>
        ///
        /// <value>The copyright.</value>
		public string Copyright { get; set; }

        /// <summary>Gets or sets the publishers.</summary>
        ///
        /// <value>The publishers.</value>
		public List<string> Publishers { get; set; }

        /// <summary>Gets or sets the genres.</summary>
        ///
        /// <value>The genres.</value>
		public List<string> Genres { get; set; }

        /// <summary>Gets or sets the sub genres.</summary>
        ///
        /// <value>The sub genres.</value>
		public List<string> SubGenres { get; set; }

        /// <summary>Gets or sets the review.</summary>
        ///
        /// <value>The review.</value>
		public string Review { get; set; }

        /// <summary>Gets or sets the review author.</summary>
        ///
        /// <value>The review author.</value>
		public string ReviewAuthor { get; set; }

        /// <summary>Gets or sets the lyrics.</summary>
        ///
        /// <value>The lyrics.</value>
		public string Lyrics { get; set; }

        /// <summary>Gets or sets the participants.</summary>
        ///
        /// <value>The participants.</value>
		public List<Participant> Participants { get; set; }

        /// <summary>Gets or sets the label.</summary>
        ///
        /// <value>The label.</value>
		public Label Label { get; set; }

        /// <summary>Gets or sets the release.</summary>
        ///
        /// <value>The release.</value>
		public Release Release { get; set; }

        /// <summary>Gets or sets the artists.</summary>
        ///
        /// <value>The artists.</value>
		public List<Artist> Artists { get; set; }

        /// <summary>Gets or sets the products.</summary>
        ///
        /// <value>The products.</value>
		public List<Product> Products { get; set; }

        /// <summary>Gets or sets the assets.</summary>
        ///
        /// <value>The assets.</value>
		public List<Asset> Assets { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        ///
        /// <value>The created date.</value>
		public DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the modified date.</summary>
        ///
        /// <value>The modified date.</value>
		public DateTime ModifiedDate { get; set; }

        /// <summary>Gets or sets the deleted date.</summary>
        ///
        /// <value>The deleted date.</value>
		public DateTime? DeletedDate { get; set; }

        /// <summary>Gets primary genre.</summary>
        ///
        /// <returns>The primary genre.</returns>
		public string GetPrimaryGenre()
		{
			return this.Genres.IsNullOrEmpty() ? null : this.Genres[0];
		}

        /// <summary>Gets primary artist.</summary>
        ///
        /// <returns>The primary artist.</returns>
		public Artist GetPrimaryArtist()
		{
			return this.Artists.IsNullOrEmpty() ? null : this.Artists[0];
		}

        /// <summary>Gets image asset identifiers.</summary>
        ///
        /// <returns>The image asset identifiers.</returns>
		public List<Asset> GetImageAssetIds()
		{
			return this.Assets.Where(x => x.AssetType.IsImage()).ToList();
		}

        /// <summary>Gets audio asset identifiers.</summary>
        ///
        /// <returns>The audio asset identifiers.</returns>
		public List<Asset> GetAudioAssetIds()
		{
			return this.Assets.Where(x => x.AssetType.IsAudio()).ToList();
		}

        /// <summary>Gets a product.</summary>
        ///
        /// <param name="territoryCode">The territory code.</param>
        ///
        /// <returns>The product.</returns>
		public Product GetProduct(string territoryCode)
		{
			if (this.Products.IsNullOrEmpty())
			{
				return null;
			}

			var product = this.Products.FirstOrDefault(x => x.TerritoryCode.EqualsIgnoreCase(territoryCode));
			if (product == null)
			{
				// Default to the world product if exists
				product = this.Products.FirstOrDefault(x => x.TerritoryCode.EqualsIgnoreCase(Release.WorldTerritoryCode));
			}

			return product;
		}

        /// <summary>Gets the description.</summary>
        ///
        /// <returns>The description.</returns>
		public string GetDescription()
		{
			return String.Format("{0} {1} ({2}) by {3}", this.SequenceNumber, this.Name, this.ReleaseText, this.ArtistText);
		}

        /// <summary>Returns a string that represents the current object.</summary>
        ///
        /// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("{0} {1}", this.Name, this.Id.ToString("N"));
		}
	}

    /// <summary>Values that represent AssetType.</summary>
	public enum AssetType
	{
        /// <summary>An enum constant representing the none option.</summary>
		None,

        /// <summary>An enum constant representing the master cover art option.</summary>
		MasterCoverArt,

        /// <summary>An enum constant representing the master artist art option.</summary>
		MasterArtistArt,

        /// <summary>An enum constant representing the master label art option.</summary>
		MasterLabelArt,

        /// <summary>An enum constant representing the resized cover art option.</summary>
		ResizedCoverArt,

        /// <summary>An enum constant representing the resized artist art option.</summary>
		ResizedArtistArt,

        /// <summary>An enum constant representing the resized label art option.</summary>
		ResizedLabelArt,

        /// <summary>An enum constant representing the track product option.</summary>
		TrackProduct,

        /// <summary>An enum constant representing the track preview option.</summary>
		TrackPreview,
	}

    /// <summary>An asset type extensions.</summary>
	public static class AssetTypeExtensions
	{
        /// <summary>An AssetType extension method that query if 'assetType' is audio.</summary>
        ///
        /// <param name="assetType">The assetType to act on.</param>
        ///
        /// <returns>true if audio, false if not.</returns>
		public static bool IsAudio(this AssetType assetType)
		{
			return assetType == AssetType.TrackProduct || assetType == AssetType.TrackPreview;
		}

        /// <summary>An AssetType extension method that query if 'assetType' is master image.</summary>
        ///
        /// <param name="assetType">The assetType to act on.</param>
        ///
        /// <returns>true if master image, false if not.</returns>
		public static bool IsMasterImage(this AssetType assetType)
		{
			return assetType == AssetType.MasterCoverArt || assetType == AssetType.MasterArtistArt || assetType == AssetType.MasterLabelArt;
		}

        /// <summary>An AssetType extension method that query if 'assetType' is resized image.</summary>
        ///
        /// <param name="assetType">The assetType to act on.</param>
        ///
        /// <returns>true if resized image, false if not.</returns>
		public static bool IsResizedImage(this AssetType assetType)
		{
			return assetType == AssetType.ResizedCoverArt || assetType == AssetType.ResizedArtistArt || assetType == AssetType.ResizedLabelArt;
		}

        /// <summary>An AssetType extension method that query if 'assetType' is image.</summary>
        ///
        /// <param name="assetType">The assetType to act on.</param>
        ///
        /// <returns>true if image, false if not.</returns>
		public static bool IsImage(this AssetType assetType)
		{
			switch (assetType)
			{
				case AssetType.MasterCoverArt:
				case AssetType.MasterArtistArt:
				case AssetType.MasterLabelArt:
				case AssetType.ResizedCoverArt:
				case AssetType.ResizedArtistArt:
				case AssetType.ResizedLabelArt:
					return true;
				default:
					return false;
			}
		}

        /// <summary>An AssetType extension method that gets master type.</summary>
        ///
        /// <param name="assetType">The assetType to act on.</param>
        ///
        /// <returns>The master type.</returns>
		public static AssetType GetMasterType(this AssetType assetType)
		{
			switch (assetType)
			{
				case AssetType.ResizedCoverArt:
					return AssetType.MasterCoverArt;
				case AssetType.ResizedArtistArt:
					return AssetType.MasterArtistArt;
				case AssetType.ResizedLabelArt:
					return AssetType.MasterLabelArt;
				default:
					return assetType;
			}
		}

        /// <summary>An AssetType extension method that gets resized type.</summary>
        ///
        /// <param name="assetType">The assetType to act on.</param>
        ///
        /// <returns>The resized type.</returns>
		public static AssetType GetResizedType(this AssetType assetType)
		{
			switch (assetType)
			{
				case AssetType.MasterCoverArt:
					return AssetType.ResizedCoverArt;
				case AssetType.MasterArtistArt:
					return AssetType.ResizedArtistArt;
				case AssetType.MasterLabelArt:
					return AssetType.ResizedLabelArt;
				default:
					return assetType;
			}
		}
	}

    /// <summary>An urn.</summary>
	public struct Urn
	{
		private const char IdValueSeperator = '/';
		private readonly string urnString;

        /// <summary>Gets the name of the resource.</summary>
        ///
        /// <value>The name of the resource.</value>
		public string ResourceName
		{
			get;
			private set;
		}

        /// <summary>Gets the name of the identifier type.</summary>
        ///
        /// <value>The name of the identifier type.</value>
		public string IdTypeName
		{
			get;
			private set;
		}

        /// <summary>Gets the identifier value.</summary>
        ///
        /// <value>The identifier value.</value>
		public string IdValue
		{
			get;
			private set;
		}

        /// <summary>Gets the identifier values.</summary>
        ///
        /// <value>The identifier values.</value>
		public string[] IdValues
		{
			get
			{
				return IdValue.Split(IdValueSeperator);
			}
		}

        /// <summary>Initializes a new instance of the DdnContentIngest class.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="idTypeName">  Name of the identifier type.</param>
        /// <param name="idValue">     The identifier value.</param>
		public Urn(string resourceName, string idTypeName, string idValue)
			: this()
		{
			if (resourceName == null)
			{
				throw new ArgumentNullException("resourceName");
			}

			if (idValue == null)
			{
				throw new ArgumentNullException("idValue");
			}

			this.ResourceName = resourceName.ToLower();
			this.IdTypeName = !String.IsNullOrEmpty(idTypeName) ? idTypeName.ToLower() : null;
			this.IdValue = idValue;

			if (String.IsNullOrEmpty(this.IdTypeName))
			{
				this.urnString = string.Format("urn:{0}:{1}", this.ResourceName, this.IdValue);
			}
			else
			{
				this.urnString = string.Format("urn:{0}:{1}:{2}", this.ResourceName, this.IdTypeName, this.IdValue);
			}
		}

        /// <summary>Initializes a new instance of the DdnContentIngest class.</summary>
        ///
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="idValue">     The identifier value.</param>
		public Urn(string resourceName, string idValue)
			: this(resourceName, null, idValue)
		{
		}

        /// <summary>Query if this object is default identifier type.</summary>
        ///
        /// <returns>true if default identifier type, false if not.</returns>
		public bool IsDefaultIdType()
		{
			return String.IsNullOrEmpty(IdTypeName);
		}

        /// <summary>Query if 'resourceName' is resource type.</summary>
        ///
        /// <param name="resourceName">Name of the resource.</param>
        ///
        /// <returns>true if resource type, false if not.</returns>
		public bool IsResourceType(string resourceName)
		{
			return string.Compare(this.ResourceName, resourceName, true) == 0;
		}

        /// <summary>Query if 'idTypeName' is identifier type.</summary>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>true if identifier type, false if not.</returns>
		public bool IsIdType(Type type)
		{
			return this.IsIdType(type.Name);
		}

        /// <summary>Query if 'idTypeName' is identifier type.</summary>
        ///
        /// <param name="idTypeName">Name of the identifier type.</param>
        ///
        /// <returns>true if identifier type, false if not.</returns>
		public bool IsIdType(string idTypeName)
		{
			return string.Compare(this.IdTypeName, idTypeName, true) == 0;
		}

        /// <summary>Returns the hash code for this instance.</summary>
        ///
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return this.urnString.GetHashCode();
		}

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        ///
        /// <param name="obj">Another object to compare to.</param>
        ///
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
		public bool Equals(Urn obj)
		{
			return String.CompareOrdinal(obj.urnString, this.urnString) == 0;
		}

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        ///
        /// <param name="obj">Another object to compare to.</param>
        ///
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			return obj.GetType() == typeof(Urn) && Equals(obj);
		}

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        ///
        /// <returns>A <see cref="T:System.String" /> containing a fully qualified type name.</returns>
		public override string ToString()
		{
			return urnString;
		}

		// Operators overloading

		public static implicit operator string(Urn urn)
		{
			return urn.urnString;
		}

        /// <summary>Equality operator.</summary>
        ///
        /// <param name="urn1">The first URN.</param>
        /// <param name="urn2">The second URN.</param>
        ///
        /// <returns>The result of the operation.</returns>
		public static bool operator ==(Urn urn1, Urn urn2)
		{
			return urn1.Equals(urn2);
		}

        /// <summary>Inequality operator.</summary>
        ///
        /// <param name="urn1">The first URN.</param>
        /// <param name="urn2">The second URN.</param>
        ///
        /// <returns>The result of the operation.</returns>
		public static bool operator !=(Urn urn1, Urn urn2)
		{
			return !urn1.Equals(urn2);
		}

		// Parsing

		public static bool IsValidUrn(string urnString)
		{
			var fields = urnString.Split(':');
			return (fields.Length == 3 || fields.Length == 4) && String.CompareOrdinal(fields[0], "urn") == 0;
		}

        /// <summary>Attempts to parse from the given data.</summary>
        ///
        /// <param name="urnString">The URN string.</param>
        /// <param name="urn">      The URN.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public static bool TryParse(string urnString, out Urn urn)
		{
			var fields = urnString.Split(':');

			if ((fields.Length == 3 || fields.Length == 4) && String.CompareOrdinal(fields[0], "urn") == 0)
			{
				if (fields.Length == 4)
				{
					urn = new Urn(fields[1], fields[2], fields[3]);
				}
				else
				{
					urn = new Urn(fields[1], fields[2]);
				}

				return true;
			}

			urn = new Urn();
			return false;
		}

        /// <summary>Parses.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="FormatException">      Thrown when the format of the ? is incorrect.</exception>
        ///
        /// <param name="urnText">The URN text.</param>
        ///
        /// <returns>An URN.</returns>
		public static Urn Parse(string urnText)
		{
			if (String.IsNullOrEmpty(urnText))
			{
				throw new ArgumentNullException("urnText");
			}

			var fields = urnText.Split(':');
			if ((fields.Length == 3 || fields.Length == 4) && String.CompareOrdinal(fields[0], "urn") == 0)
			{
				return fields.Length == 4 ? new Urn(fields[1], fields[2], fields[3]) : new Urn(fields[1], fields[2]);
			}

			var msg = string.Format("Invalid URN text '{0}'", urnText);
			throw new FormatException(msg);
		}

        /// <summary>Gets URN type.</summary>
        ///
        /// <param name="urnString">The URN string.</param>
        ///
        /// <returns>The URN type.</returns>
		public static string GetUrnType(string urnString)
		{
			var urn = Parse(urnString);
			return urn.ResourceName;
		}

        /// <summary>Gets long identifier.</summary>
        ///
        /// <param name="urnString">The URN string.</param>
        ///
        /// <returns>The long identifier.</returns>
		public static long GetLongId(string urnString)
		{
			var urn = Parse(urnString);
			return Convert.ToInt64(urn.IdValue);
		}

        /// <summary>Gets unique identifier.</summary>
        ///
        /// <param name="urnString">The URN string.</param>
        ///
        /// <returns>The unique identifier.</returns>
		public static Guid GetGuidId(string urnString)
		{
			var urn = Parse(urnString);
			return new Guid(urn.IdValue);
		}

        /// <summary>Gets identifier values.</summary>
        ///
        /// <param name="urnString">The URN string.</param>
        ///
        /// <returns>An array of string.</returns>
		public static string[] GetIdValues(string urnString)
		{
			var urn = Parse(urnString);
			return urn.IdValues;
		}

        /// <summary>Gets identifier value.</summary>
        ///
        /// <param name="urnString">The URN string.</param>
        ///
        /// <returns>The identifier value.</returns>
		public static string GetIdValue(string urnString)
		{
			var urn = Parse(urnString);
			return urn.IdValue;
		}

        /// <summary>Clean identifier value.</summary>
        ///
        /// <param name="idValue">The identifier value.</param>
        ///
        /// <returns>A string.</returns>
		public static string CleanIdValue(string idValue)
		{
			return idValue.Trim().ToLowerInvariant().Replace(' ', '_');
		}

        /// <summary>Gets the first identifier value.</summary>
        ///
        /// <param name="urnString">The URN string.</param>
        ///
        /// <returns>The first identifier value.</returns>
		public static string GetFirstIdValue(string urnString)
		{
			var urn = Parse(urnString);
			return urn.IdValue.Split('/')[0];
		}

        /// <summary>Gets second identifier value.</summary>
        ///
        /// <param name="urnString">The URN string.</param>
        ///
        /// <returns>The second identifier value.</returns>
		public static string GetSecondIdValue(string urnString)
		{
			var urn = Parse(urnString);
			var parts = urn.IdValue.Split('/');
			return parts.Length > 1 ? parts[1] : null;
		}
	}

}