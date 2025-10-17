using System;
using System.Collections.Generic;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Models;

public partial class BASS_DBContext : DbContext
{
    public BASS_DBContext()
    {
    }

    public BASS_DBContext(DbContextOptions<BASS_DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminInfo> AdminInfos { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<CategMaster> CategMasters { get; set; }

    public virtual DbSet<ClientCity> ClientCities { get; set; }

    public virtual DbSet<ClientMaster> ClientMasters { get; set; }

    public virtual DbSet<CommCharge> CommCharges { get; set; }
    public virtual DbSet<Division> Divisions { get; set; }

    public virtual DbSet<DelDetl> DelDetls { get; set; }

    public virtual DbSet<DelHead> DelHeads { get; set; }

    public virtual DbSet<GenCity> GenCities { get; set; }

    public virtual DbSet<GenCountry> GenCountries { get; set; }
	public virtual DbSet<GenCurrency> GenCurrencies { get; set; }

	public virtual DbSet<GenScanSpec> GenScanSpecs { get; set; }

    public virtual DbSet<GroupMaster> GroupMasters { get; set; }

    public virtual DbSet<ItemMaster> ItemMasters { get; set; }

    public virtual DbSet<ItemUnit> ItemUnits { get; set; }

    public virtual DbSet<MenuInfo> MenuInfos { get; set; }
	public virtual DbSet<PoDetail> PoDetails { get; set; }
	public virtual DbSet<PoHead> PoHeads { get; set; }

	public virtual DbSet<Productcode> Productcodes { get; set; }

    public virtual DbSet<RcptDetail> RcptDetails { get; set; }

    public virtual DbSet<RcptHead> RcptHeads { get; set; }

    public virtual DbSet<RoleInfo> RoleInfos { get; set; }

    public virtual DbSet<SdelDetl> SdelDetls { get; set; }

    public virtual DbSet<SdelHead> SdelHeads { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }
    public virtual DbSet<StockCheck> StockChecks { get; set; }

    public virtual DbSet<StockForJournal> StockForJournals { get; set; }

    public virtual DbSet<StockTran> StockTrans { get; set; }

    public virtual DbSet<SupplierMaster> SupplierMasters { get; set; }

    public virtual DbSet<SysPagesControl> SysPagesControls { get; set; }

    public virtual DbSet<SystemPage> SystemPages { get; set; }

    public virtual DbSet<TrDetail> TrDetails { get; set; }

    public virtual DbSet<TrHead> TrHeads { get; set; }

    public virtual DbSet<VwBalQty> VwBalQties { get; set; }
    public virtual DbSet<VwItemBal> VwItemBals { get; set; }

    public virtual DbSet<VwReceipt> VwReceipts { get; set; }
	public virtual DbSet<VwSale> VwSales { get; set; }
    public virtual DbSet<VwStockForPriceUpd> VwStockForPriceUpds { get; set; }
    public virtual DbSet<VwStockForSpriceUpd> VwStockForSpriceUpds { get; set; }

    public virtual DbSet<VwTransfer> VwTransfers { get; set; }
    public virtual DbSet<VwPurchaseOrder> VwPurchaseOrders { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RcptDetail>().Ignore(p => p.PohList);

        modelBuilder.Entity<AdminInfo>(entity =>
        {
            entity.ToTable("AdminInfo");

            entity.HasIndex(e => e.Email, "IX_AdminInfo").IsUnique();

            entity.Property(e => e.CreatedOn).HasMaxLength(25);
            entity.Property(e => e.Email).HasMaxLength(30);
            entity.Property(e => e.LastLogin).HasMaxLength(25);
            entity.Property(e => e.LocId)
                .HasMaxLength(3)
                .HasColumnName("Loc_Id");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(15);
            entity.Property(e => e.RoleId)
                .HasMaxLength(2)
                .HasColumnName("Role_Id");
            entity.Property(e => e.UpdatedOn).HasMaxLength(25);
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.ToTable("Branch");

            entity.HasIndex(e => e.BranchCode, "IX_Branch").IsUnique();

            entity.Property(e => e.BranchId)
                .ValueGeneratedNever()
                .HasComment("")
                .HasColumnName("Branch_Id");
            entity.Property(e => e.BranchAddress)
                .HasMaxLength(250)
                .HasColumnName("Branch_Address");
            entity.Property(e => e.BranchAdminId).HasColumnName("Branch_AdminId");
            entity.Property(e => e.BranchCity)
                .HasMaxLength(50)
                .HasColumnName("Branch_City");
            entity.Property(e => e.BranchCode)
                .HasMaxLength(2)
                .HasColumnName("Branch_Code");
            entity.Property(e => e.BranchCountry)
                .HasMaxLength(30)
                .HasColumnName("Branch_Country");
            entity.Property(e => e.BranchCrNo)
                .HasMaxLength(20)
                .HasColumnName("Branch_CrNo");
            entity.Property(e => e.BranchDesc)
                .HasMaxLength(50)
                .HasColumnName("Branch_Desc");
            entity.Property(e => e.BranchDescArb)
                .HasMaxLength(75)
                .HasColumnName("Branch_Desc_Arb");
            entity.Property(e => e.BranchEmail)
                .HasMaxLength(60)
                .HasColumnName("Branch_Email");
            entity.Property(e => e.BranchPhone)
                .HasMaxLength(50)
                .HasColumnName("Branch_Phone");
            entity.Property(e => e.BranchPoBox)
                .HasMaxLength(20)
                .HasColumnName("Branch_PoBox");
            entity.Property(e => e.BranchState)
                .HasMaxLength(30)
                .HasColumnName("Branch_State");
            entity.Property(e => e.BranchUrl)
                .HasMaxLength(100)
                .HasColumnName("Branch_Url");
            entity.Property(e => e.BranchVatNo)
                .HasMaxLength(20)
                .HasColumnName("Branch_VatNo");
            entity.Property(e => e.SetupPassword)
                .HasMaxLength(20)
                .HasColumnName("Setup_Password");
            entity.HasOne(d => d.BranchNavigation).WithOne(p => p.Branch)
                .HasForeignKey<Branch>(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Branch_AdminInfo");
        });

        modelBuilder.Entity<CategMaster>(entity =>
        {
            entity.HasKey(e => e.CatId);

            entity.ToTable("Categ_Master");

            entity.HasIndex(e => new { e.CatGrpNo, e.CatNo }, "IX_Categ_Master").IsUnique();

            entity.Property(e => e.CatId).HasColumnName("Cat_Id");
            entity.Property(e => e.CatDesc)
                .HasMaxLength(30)
                .HasColumnName("Cat_Desc");
            entity.Property(e => e.CatGrpNo)
                .HasMaxLength(10)
                .HasColumnName("Cat_GrpNo");
            entity.Property(e => e.CatNo)
                .HasMaxLength(10)
                .HasColumnName("Cat_No");
            entity.Property(e => e.CatShortDesc)
                .HasMaxLength(10)
                .HasColumnName("Cat_ShortDesc");

            entity.HasOne(d => d.CatGrpNoNavigation).WithMany(p => p.CategMasters)
                .HasPrincipalKey(p => p.GrpNo)
                .HasForeignKey(d => d.CatGrpNo)
                .HasConstraintName("FK_Categ_Master_Group_Master");
        });

        modelBuilder.Entity<ClientCity>(entity =>
        {
            entity.ToTable("Client_City");

            entity.HasIndex(e => new { e.ClientId, e.CityId }, "IX_Client_City").IsUnique();
        });

        modelBuilder.Entity<ClientMaster>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK_Client_Master_1");

            entity.ToTable("Client_Master");

            entity.HasIndex(e => e.ClientCode, "IX_Client_Master").IsUnique();

            entity.Property(e => e.ClientId).HasColumnName("Client_Id");
            entity.Property(e => e.ClientAddr)
                .HasMaxLength(300)
                .HasColumnName("Client_Addr");
            entity.Property(e => e.ClientCode)
                .HasMaxLength(8)
                .HasColumnName("Client_Code");
            entity.Property(e => e.ClientContactPerson)
                .HasMaxLength(150)
                .HasColumnName("Client_Contact_Person");
            entity.Property(e => e.ClientCrNumber)
                .HasMaxLength(20)
                .HasColumnName("Client_CR_Number");
            entity.Property(e => e.ClientEmail)
                .HasMaxLength(100)
                .HasColumnName("Client_Email");
            entity.Property(e => e.ClientName)
                .HasMaxLength(150)
                .HasColumnName("Client_Name");
            entity.Property(e => e.ClientRemarks).HasColumnName("Client_Remarks");
            entity.Property(e => e.ClientTel)
                .HasMaxLength(60)
                .HasColumnName("Client_Tel");
            entity.Property(e => e.ClientUrl)
                .HasMaxLength(100)
                .HasColumnName("Client_Url");
            entity.Property(e => e.ClientVendCode)
                .HasMaxLength(10)
                .HasColumnName("Client_Vend_Code");
            entity.Property(e => e.ClientZohoClientId)
                .HasMaxLength(25)
                .HasColumnName("Client_Zoho_Client_Id");
        });

        modelBuilder.Entity<CommCharge>(entity =>
        {
            entity.HasKey(e => e.CommId);

            entity.Property(e => e.CommAmt).HasColumnType("numeric(11, 2)");
            entity.Property(e => e.CommDate).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<Division>(entity =>
        {
            entity.HasKey(e => e.LocId);

            entity.ToTable("Division");

            entity.HasIndex(e => e.LocCode, "IX_Division").IsUnique();

            entity.Property(e => e.LocId).HasColumnName("Loc_Id");
            entity.Property(e => e.LocAddress)
                .HasMaxLength(250)
                .HasColumnName("Loc_Address");
            entity.Property(e => e.LocBranchCode)
                .HasMaxLength(2)
                .HasColumnName("Loc_Branch_Code");
            entity.Property(e => e.LocCity)
                .HasMaxLength(30)
                .HasColumnName("Loc_City");
            entity.Property(e => e.LocCode)
                .HasMaxLength(3)
                .HasColumnName("Loc_Code");
            entity.Property(e => e.LocDesc)
                .HasMaxLength(50)
                .HasColumnName("Loc_Desc");
            entity.Property(e => e.LocState)
                .HasMaxLength(30)
                .HasColumnName("Loc_State");

            entity.HasOne(d => d.LocBranchCodeNavigation).WithMany(p => p.Divisions)
                .HasPrincipalKey(p => p.BranchCode)
                .HasForeignKey(d => d.LocBranchCode)
                .HasConstraintName("FK_Division_Division");
        });

        modelBuilder.Entity<DelDetl>(entity =>
        {
            entity.HasKey(e => e.DelDetId);

            entity.ToTable("Del_Detl");

            entity.Property(e => e.DelDetId).HasColumnName("Del_DetId");
            entity.Property(e => e.DelBatchId).HasColumnName("Del_BatchId");
            entity.Property(e => e.DelClientCode)
                .HasMaxLength(8)
                .HasColumnName("Del_ClientCode");
            entity.Property(e => e.DelExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Del_ExpiryDate");
            entity.Property(e => e.DelListNo)
                .HasMaxLength(10)
                .HasColumnName("Del_ListNo");
            entity.Property(e => e.DelLotNo)
                .HasMaxLength(10)
                .HasColumnName("Del_LotNo");
            entity.Property(e => e.DelPurchPrice)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Del_PurchPrice");
            entity.Property(e => e.DelQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Del_Qty");
            entity.Property(e => e.DelScanCode)
                .HasMaxLength(50)
                .HasColumnName("Del_ScanCode");
            entity.Property(e => e.DelStkId).HasColumnName("Del_StkId");
            entity.Property(e => e.DelStkIdCat)
                .HasMaxLength(10)
                .HasColumnName("Del_StkId_Cat");
            entity.Property(e => e.DelStkIdDesc).HasColumnName("Del_StkId_Desc");
            entity.Property(e => e.DelStkIdGrp)
                .HasMaxLength(10)
                .HasColumnName("Del_StkId_Grp");
            entity.Property(e => e.DelStkIdUnit)
                .HasMaxLength(5)
                .HasColumnName("Del_StkId_Unit");
            entity.Property(e => e.DelStockStkId).HasColumnName("Del_Stock_StkId");
            entity.Property(e => e.DelUprice)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Del_Uprice");

            entity.HasOne(d => d.DelHead).WithMany(p => p.DelDetls)
                .HasForeignKey(d => d.DelHeadId)
                .HasConstraintName("FK_Del_Detl_Del_Head");
        });

        modelBuilder.Entity<DelHead>(entity =>
        {
            entity.HasKey(e => e.DelId);

            entity.ToTable("Del_Head");

            entity.HasIndex(e => e.DelNo, "IX_Del_Head").IsUnique();

            entity.Property(e => e.DelId).HasColumnName("Del_Id");
            entity.Property(e => e.DelApproved)
                .HasDefaultValue(false)
                .HasColumnName("Del_Approved");
            entity.Property(e => e.DelBranch)
                .HasMaxLength(3)
                .HasColumnName("Del_Branch");
            entity.Property(e => e.DelComp)
                .HasMaxLength(2)
                .HasColumnName("Del_Comp");
            entity.Property(e => e.DelCityId).HasColumnName("Del_CityId");
            entity.Property(e => e.DelClientId).HasColumnName("Del_ClientId");
            entity.Property(e => e.DelDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Del_Date");
            entity.Property(e => e.DelDateAltered)
                .HasColumnType("smalldatetime")
                .HasColumnName("Del_Date_Altered");
            entity.Property(e => e.DelDispNo)
                .HasMaxLength(20)
                .HasColumnName("Del_Disp_No");
            entity.Property(e => e.DelNo).HasColumnName("Del_No");
            entity.Property(e => e.DelUser)
                .HasMaxLength(30)
                .HasColumnName("Del_User");
            entity.Property(e => e.PoDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Po_Date");
            entity.Property(e => e.PoNumber)
                .HasMaxLength(50)
                .HasColumnName("Po_Number");
        });

        modelBuilder.Entity<GenCity>(entity =>
        {
            entity.HasKey(e => e.CityId);

            entity.ToTable("Gen_City");

            entity.HasIndex(e => new { e.CityCountryCode, e.CityCode }, "IX_Gen_City").IsUnique();

            entity.Property(e => e.CityId).HasColumnName("City_Id");
            entity.Property(e => e.CityCode)
                .HasMaxLength(5)
                .HasColumnName("City_Code");
            entity.Property(e => e.CityCountryCode)
                .HasMaxLength(5)
                .HasColumnName("City_CountryCode");
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .HasColumnName("City_Name");

            entity.HasOne(d => d.CityCountryCodeNavigation).WithMany(p => p.GenCities)
                .HasPrincipalKey(p => p.CountryCode)
                .HasForeignKey(d => d.CityCountryCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gen_City_Gen_Country");
        });

        modelBuilder.Entity<GenCountry>(entity =>
        {
            entity.HasKey(e => e.CountryId);

            entity.ToTable("Gen_Country");

            entity.HasIndex(e => e.CountryCode, "IX_Gen_Country").IsUnique();

            entity.Property(e => e.CountryId).HasColumnName("Country_Id");
            entity.Property(e => e.CountryCode)
                .HasMaxLength(5)
                .HasColumnName("Country_Code");
            entity.Property(e => e.CountryName)
                .HasMaxLength(50)
                .HasColumnName("Country_Name");
        });

		modelBuilder.Entity<GenCurrency>(entity =>
		{
			entity.HasKey(e => e.CurrId);

			entity.ToTable("Gen_Currency");

			entity.HasIndex(e => e.CurrShortName, "IX_Gen_Currency").IsUnique();

			entity.Property(e => e.CurrId).HasColumnName("Curr_Id");
			entity.Property(e => e.CurrLongName)
				.HasMaxLength(50)
				.HasColumnName("Curr_LongName");
			entity.Property(e => e.CurrShortName)
				.HasMaxLength(5)
				.HasColumnName("Curr_ShortName");
		});

		modelBuilder.Entity<GenScanSpec>(entity =>
        {
            entity.HasKey(e => e.GenId);

            entity.ToTable("Gen_ScanSpec");

            entity.HasIndex(e => e.GenScanLength, "IX_Gen_ScanSpec");

            entity.Property(e => e.GenId).HasColumnName("gen_Id");
            entity.Property(e => e.GenExpiryDir).HasColumnName("gen_ExpiryDir");
            entity.Property(e => e.GenExpiryLength).HasColumnName("gen_ExpiryLength");
            entity.Property(e => e.GenExpiryStartFrom).HasColumnName("gen_ExpiryStartFrom");
            entity.Property(e => e.GenListLength).HasColumnName("gen_ListLength");
            entity.Property(e => e.GenListStartFrom).HasColumnName("gen_ListStartFrom");
            entity.Property(e => e.GenLotLength).HasColumnName("gen_LotLength");
            entity.Property(e => e.GenLotStartFrom).HasColumnName("gen_LotStartFrom");
            entity.Property(e => e.GenScanLength).HasColumnName("gen_ScanLength");
            entity.Property(e => e.GenType).HasColumnName("gen_type");
        });

        modelBuilder.Entity<GroupMaster>(entity =>
        {
            entity.HasKey(e => e.GrpId);

            entity.ToTable("Group_Master");

            entity.HasIndex(e => e.GrpNo, "IX_Group_Master").IsUnique();

            entity.Property(e => e.GrpId).HasColumnName("Grp_Id");
            entity.Property(e => e.GrpDesc)
                .HasMaxLength(30)
                .HasColumnName("Grp_Desc");
            entity.Property(e => e.GrpNo)
                .HasMaxLength(10)
                .HasColumnName("Grp_No");
            entity.Property(e => e.GrpShortDesc)
                .HasMaxLength(10)
                .HasColumnName("Grp_ShortDesc");
        });

        modelBuilder.Entity<ItemMaster>(entity =>
        {
            entity.HasKey(e => e.ItemId);

            entity.ToTable("Item_Master");

            entity.HasIndex(e => new { e.ItemListNo, e.ItemClientCode }, "IX_Item_Master").IsUnique();

            entity.Property(e => e.ItemId).HasColumnName("Item_Id");
            entity.Property(e => e.ItemCatCode)
                .HasMaxLength(10)
                .HasColumnName("Item_CatCode");
            entity.Property(e => e.ItemClientCode)
                .HasMaxLength(8)
                .HasColumnName("Item_ClientCode");
            entity.Property(e => e.ItemCostPrice)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_CostPrice");
            entity.Property(e => e.ItemCostPricePrev)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_CostPrice_Prev");
            entity.Property(e => e.ItemDesc)
                .HasMaxLength(300)
                .HasColumnName("Item_Desc");
            entity.Property(e => e.ItemGrpCode)
                .HasMaxLength(10)
                .HasColumnName("Item_GrpCode");
            entity.Property(e => e.ItemListNo)
                .HasMaxLength(10)
                .HasColumnName("Item_ListNo");
            entity.Property(e => e.ItemListNoProd)
                .HasMaxLength(20)
                .HasColumnName("Item_ListNoProd");
            entity.Property(e => e.ItemProdCode)
                .HasMaxLength(50)
                .HasColumnName("Item_ProdCode");
            entity.Property(e => e.ItemScanCode)
                .HasMaxLength(50)
                .HasColumnName("Item_ScanCode");
            entity.Property(e => e.ItemSellPrice)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_SellPrice");
            entity.Property(e => e.ItemSellPricePrev)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_SellPrice_Prev");
            entity.Property(e => e.ItemSuppCode)
                .HasMaxLength(8)
                .HasColumnName("Item_SuppCode");
            entity.Property(e => e.ItemUnit)
                .HasMaxLength(5)
                .HasColumnName("Item_Unit");
            entity.Property(e => e.ItemZohoItemId)
                .HasMaxLength(25)
                .HasColumnName("Item_Zoho_Item_Id");
            entity.Property(e => e.ScanCodeLength).HasColumnName("ScanCode_Length");
            entity.Property(e => e.ItemHsnCode)
                .HasMaxLength(10)
                .HasColumnName("Item_HsnCode");
            entity.Property(e => e.ItemGst)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(5, 2)")
                .HasColumnName("Item_Gst");
        });

        modelBuilder.Entity<ItemUnit>(entity =>
        {
            entity.ToTable("Item_Unit");

            entity.HasIndex(e => e.ItemUnitDesc, "IX_Item_Unit").IsUnique();

            entity.Property(e => e.ItemUnitId).HasColumnName("Item_UnitId");
            entity.Property(e => e.ItemUnitDesc)
                .HasMaxLength(5)
                .HasColumnName("Item_UnitDesc");
        });

        modelBuilder.Entity<MenuInfo>(entity =>
        {
            entity.HasKey(e => e.MenuId);

            entity.ToTable("MenuInfo");

            entity.Property(e => e.MenuId).ValueGeneratedNever();
            entity.Property(e => e.IconName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MenuName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PageName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Productcode>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("productcode");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DmsProductNameEnglish)
                .HasMaxLength(255)
                .HasColumnName("DMS Product Name (English)");
            entity.Property(e => e.ListNoSizeNo).HasMaxLength(255);
            entity.Property(e => e.SapItemNumber)
                .HasMaxLength(255)
                .HasColumnName("SAP Item Number");
        });

		modelBuilder.Entity<PoDetail>(entity =>
		{
			entity.HasKey(e => e.PodId);

			entity.ToTable("Po_Detail");

			entity.Property(e => e.PodId).HasColumnName("Pod_Id");
			entity.Property(e => e.PodAmount)
				.HasColumnType("numeric(11, 2)")
				.HasColumnName("Pod_Amount");
			entity.Property(e => e.PodDiscAmt)
				.HasDefaultValue(0m)
				.HasColumnType("numeric(11, 2)")
				.HasColumnName("Pod_DiscAmt");
			entity.Property(e => e.PodDiscPct)
				.HasDefaultValue(0m)
				.HasColumnType("numeric(6, 2)")
				.HasColumnName("Pod_DiscPct");
			entity.Property(e => e.PodGstPct)
				.HasDefaultValue(0m)
				.HasColumnType("numeric(5, 2)")
				.HasColumnName("Pod_Gst_Pct");
			entity.Property(e => e.PodInvdQty)
				.HasDefaultValue(0m)
				.HasColumnType("numeric(11, 2)")
				.HasColumnName("Pod_Invd_Qty");
			entity.Property(e => e.PodListNo)
				.HasMaxLength(10)
				.HasColumnName("Pod_ListNo");
			entity.Property(e => e.PodPohId).HasColumnName("Pod_PohId");
			entity.Property(e => e.PodQty)
				.HasDefaultValue(0m)
				.HasColumnType("numeric(11, 2)")
				.HasColumnName("Pod_Qty");
			entity.Property(e => e.PodRcvdQty)
				.HasDefaultValue(0m)
				.HasColumnType("numeric(11, 2)")
				.HasColumnName("Pod_Rcvd_Qty");
			entity.Property(e => e.PodRtndQty)
				.HasDefaultValue(0m)
				.HasColumnType("numeric(11, 2)")
				.HasColumnName("Pod_Rtnd_Qty");
			entity.Property(e => e.PodStkIdCat)
				.HasMaxLength(10)
				.HasColumnName("Pod_StkId_Cat");
			entity.Property(e => e.PodStkIdDesc).HasColumnName("Pod_StkId_Desc");
			entity.Property(e => e.PodStkIdGrp)
				.HasMaxLength(10)
				.HasColumnName("Pod_StkId_Grp");
			entity.Property(e => e.PodStkIdUnit)
				.HasMaxLength(5)
				.HasColumnName("Pod_StkId_Unit");
			entity.Property(e => e.PodUp)
				.HasDefaultValue(0m)
				.HasColumnType("numeric(11, 2)")
				.HasColumnName("Pod_Up");
			entity.Property(e => e.PodUpAftDisc)
				.HasDefaultValue(0m)
				.HasColumnType("numeric(11, 4)")
				.HasColumnName("Pod_Up_AftDisc");
			entity.HasOne(d => d.PodPoh).WithMany(p => p.PoDetails)
				.HasForeignKey(d => d.PodPohId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Po_Detail_Po_Head");
            entity.Property(e => e.PodHsnCode)
                .HasMaxLength(10)
                .HasColumnName("Pod_HsnCode");
        });

		modelBuilder.Entity<PoHead>(entity =>
		{
			entity.HasKey(e => e.PohId);

			entity.ToTable("Po_Head");

			entity.HasIndex(e => e.PohNo, "IX_Po_Head").IsUnique();

			entity.Property(e => e.PohId).HasColumnName("Poh_Id");
			entity.Property(e => e.PohApproved)
				.HasDefaultValue(false)
				.HasColumnName("Poh_Approved");
            entity.Property(e => e.PohBranch)
                .HasMaxLength(3)
                .HasColumnName("Poh_Branch");
            entity.Property(e => e.PohComp)
                .HasMaxLength(2)
                .HasColumnName("Poh_Comp");
            entity.Property(e => e.PohConvRate)
				.HasColumnType("numeric(8, 4)")
				.HasColumnName("Poh_ConvRate");
			entity.Property(e => e.PohCurr)
				.HasMaxLength(5)
				.HasColumnName("Poh_Curr");
			entity.Property(e => e.PohDate)
				.HasColumnType("smalldatetime")
				.HasColumnName("Poh_Date");
			entity.Property(e => e.PohDateAltered)
				.HasColumnType("smalldatetime")
				.HasColumnName("Poh_Date_Altered");
			entity.Property(e => e.PohDispNo)
				.HasMaxLength(20)
				.HasColumnName("Poh_Disp_No");
			entity.Property(e => e.PohNo).HasColumnName("Poh_No");
			entity.Property(e => e.PohRemarks).HasColumnName("Poh_Remarks");
			entity.Property(e => e.PohUser)
				.HasMaxLength(30)
				.HasColumnName("Poh_User");
            entity.Property(e => e.PohCustId).HasColumnName("Poh_CustId");
            entity.Property(e => e.PohVendId).HasColumnName("Poh_VendId");
			entity.Property(e => e.PohVendRef)
				.HasMaxLength(50)
				.HasColumnName("Poh_VendRef");

			entity.HasOne(d => d.PohCurrNavigation).WithMany(p => p.PoHeads)
				.HasPrincipalKey(p => p.CurrShortName)
				.HasForeignKey(d => d.PohCurr)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Po_Head_Gen_Currency");
		});

		modelBuilder.Entity<RcptDetail>(entity =>
        {
            entity.HasKey(e => e.RdId);

            entity.ToTable("Rcpt_Detail");

            entity.Property(e => e.RdId).HasColumnName("Rd_Id");
            entity.Property(e => e.RdClientCode)
                .HasMaxLength(8)
                .HasColumnName("Rd_ClientCode");
            entity.Property(e => e.RdExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Rd_ExpiryDate");
            entity.Property(e => e.RdListNo)
                .HasMaxLength(10)
                .HasColumnName("Rd_ListNo");
            entity.Property(e => e.RdLotNo)
                .HasMaxLength(10)
                .HasColumnName("Rd_LotNo");
            entity.Property(e => e.RdQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Rd_Qty");
            entity.Property(e => e.RdRhId).HasColumnName("Rd_RhId");
            entity.Property(e => e.RdScanCode)
                .HasMaxLength(50)
                .HasColumnName("Rd_ScanCode");
            entity.Property(e => e.RdStkId).HasColumnName("Rd_StkId");
            entity.Property(e => e.RdStkIdCat)
                .HasMaxLength(10)
                .HasColumnName("Rd_StkId_Cat");
            entity.Property(e => e.RdStkIdDesc).HasColumnName("Rd_StkId_Desc");
            entity.Property(e => e.RdStkIdGrp)
                .HasMaxLength(10)
                .HasColumnName("Rd_StkId_Grp");
            entity.Property(e => e.RdStkIdUnit)
                .HasMaxLength(5)
                .HasColumnName("Rd_StkId_Unit");
            entity.Property(e => e.RdStockStkId).HasColumnName("Rd_Stock_StkId");
            entity.Property(e => e.RdSuppCode)
                .HasMaxLength(8)
                .HasColumnName("Rd_SuppCode");
            entity.Property(e => e.RdUp)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Rd_Up");
            entity.Property(e => e.RdPohDispNo)
                .HasMaxLength(20)
                .HasColumnName("Rd_PohDispNo");
            entity.Property(e => e.RdVendInvDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Rd_Vend_InvDate");
            entity.Property(e => e.RdVendInvNo)
                .HasMaxLength(20)
                .HasColumnName("Rd_Vend_InvNo");

            entity.HasOne(d => d.RdRh).WithMany(p => p.RcptDetails)
                .HasForeignKey(d => d.RdRhId)
                .HasConstraintName("FK_Rcpt_Detail_Rcpt_Head");
        });

        modelBuilder.Entity<RcptHead>(entity =>
        {
            entity.HasKey(e => e.RhId);

            entity.ToTable("Rcpt_Head");

            entity.HasIndex(e => e.RhNo, "IX_Rcpt_Head").IsUnique();

            entity.Property(e => e.RhId).HasColumnName("Rh_Id");
            entity.Property(e => e.RhApproved)
                .HasDefaultValue(false)
                .HasColumnName("Rh_Approved");
            entity.Property(e => e.RhBranch)
                .HasMaxLength(3)
                .HasColumnName("Rh_Branch");
            entity.Property(e => e.RhComp)
                .HasMaxLength(2)
                .HasColumnName("Rh_Comp");
            entity.Property(e => e.RhDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Rh_Date");
            entity.Property(e => e.RhDateAltered)
                .HasColumnType("smalldatetime")
                .HasColumnName("Rh_Date_Altered");
            entity.Property(e => e.RhDispNo)
                .HasMaxLength(20)
                .HasColumnName("Rh_Disp_No");
            entity.Property(e => e.RhNo).HasColumnName("Rh_No");
            entity.Property(e => e.RhPoNo)
                .HasMaxLength(30)
                .HasColumnName("Rh_PoNo");
            entity.Property(e => e.RhRemarks).HasColumnName("Rh_Remarks");
            entity.Property(e => e.RhSuppId).HasColumnName("Rh_SuppId");
            entity.Property(e => e.RhUser)
                .HasMaxLength(30)
                .HasColumnName("Rh_User");
            entity.Property(e => e.RhVendDelNote)
                .HasMaxLength(20)
                .HasColumnName("Rh_Vend_DelNote");
            entity.Property(e => e.RhVendInvNote)
                .HasMaxLength(20)
                .HasColumnName("Rh_Vend_InvNote");
        });

        modelBuilder.Entity<RoleInfo>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.ToTable("RoleInfo");

            entity.Property(e => e.RoleId).HasColumnName("Role_Id");
            entity.Property(e => e.RoleCode)
                .HasMaxLength(2)
                .HasColumnName("Role_Code");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .HasColumnName("Role_Name");
        });

        modelBuilder.Entity<SdelDetl>(entity =>
        {
            entity.HasKey(e => e.SdelDetId);

            entity.ToTable("SDel_Detl");

            entity.Property(e => e.SdelDetId).HasColumnName("SDel_DetId");
            entity.Property(e => e.SdelClientCode)
                .HasMaxLength(8)
                .HasColumnName("SDel_ClientCode");
            entity.Property(e => e.SdelClientVendCode)
                .HasMaxLength(10)
                .HasColumnName("SDel_ClientVendCode");
            entity.Property(e => e.SdelDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("SDel_Date");
            entity.Property(e => e.SdelExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("SDel_ExpiryDate");
            entity.Property(e => e.SdelHeadId).HasColumnName("SDelHeadId");
            entity.Property(e => e.SdelListNo)
                .HasMaxLength(10)
                .HasColumnName("SDel_ListNo");
            entity.Property(e => e.SdelListNoProd)
                .HasMaxLength(20)
                .HasColumnName("Sdel_ListNoProd");
            entity.Property(e => e.SdelLotNo)
                .HasMaxLength(10)
                .HasColumnName("SDel_LotNo");
            entity.Property(e => e.SdelProdCode)
                .HasMaxLength(20)
                .HasColumnName("SDel_ProdCode");
            entity.Property(e => e.SdelQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("SDel_Qty");
            entity.Property(e => e.SdelStkIdDesc).HasColumnName("SDel_StkId_Desc");
            entity.Property(e => e.SdelUprice)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("SDel_Uprice");

            entity.HasOne(d => d.SdelHead).WithMany(p => p.SdelDetls)
                .HasForeignKey(d => d.SdelHeadId)
                .HasConstraintName("FK_SDel_Detl_SDel_Head");
        });

        modelBuilder.Entity<SdelHead>(entity =>
        {
            entity.HasKey(e => e.SdelId);

            entity.ToTable("SDel_Head");

            entity.HasIndex(e => e.SdelNo, "IX_SDel_Head").IsUnique();

            entity.Property(e => e.SdelId).HasColumnName("SDel_Id");
            entity.Property(e => e.SdelApproved).HasColumnName("Sdel_Approved");
            entity.Property(e => e.SdelDate)
                .HasComment("Transaction Date")
                .HasColumnType("smalldatetime")
                .HasColumnName("SDel_Date");
            entity.Property(e => e.SdelDateAltered)
                .HasColumnType("smalldatetime")
                .HasColumnName("SDel_Date_Altered");
            entity.Property(e => e.SdelBranch)
                .HasMaxLength(3)
                .HasColumnName("SDel_Branch");
            entity.Property(e => e.SdelComp)
                .HasMaxLength(2)
                .HasColumnName("SDel_Comp");
            entity.Property(e => e.SdelDateFrom)
                .HasColumnType("smalldatetime")
                .HasColumnName("SDel_DateFrom");
            entity.Property(e => e.SdelDateTo)
                .HasColumnType("smalldatetime")
                .HasColumnName("SDel_DateTo");
            entity.Property(e => e.SdelDispNo)
                .HasMaxLength(20)
                .HasColumnName("SDel_Disp_No");
            entity.Property(e => e.SdelNo)
                .HasComment("Transaction Number")
                .HasColumnName("SDel_No");
            entity.Property(e => e.SdelUser)
                .HasMaxLength(30)
                .HasColumnName("SDel_User");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.StkId);

            entity.ToTable("Stock");

            entity.HasIndex(e => new { e.ItemLotNo, e.ItemExpiryDate, e.ItemListNo, e.ItemClientCode }, "IX_Stock").IsUnique();

            entity.Property(e => e.StkId).HasColumnName("Stk_Id");
            entity.Property(e => e.ItemBatchId).HasColumnName("Item_BatchId");
            entity.Property(e => e.ItemClientCode)
                .HasMaxLength(8)
                .HasColumnName("Item_ClientCode");
            entity.Property(e => e.ItemCp)
                .HasDefaultValue(0.00m)
                .HasComment("Cost Price")
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Cp");
            entity.Property(e => e.ItemDelAmt)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Del_Amt");
            entity.Property(e => e.ItemDelQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Del_Qty");
            entity.Property(e => e.ItemExpStat)
                .HasMaxLength(3)
                .HasDefaultValueSql("((0))")
                .HasColumnName("Item_ExpStat");
            entity.Property(e => e.ItemExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Item_ExpiryDate");
            entity.Property(e => e.ItemId).HasColumnName("Item_Id");
            entity.Property(e => e.ItemListNo)
                .HasMaxLength(10)
                .HasColumnName("Item_ListNo");
            entity.Property(e => e.ItemLotNo)
                .HasMaxLength(10)
                .HasColumnName("Item_LotNo");
            entity.Property(e => e.ItemOpQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_OpQty");
            entity.Property(e => e.ItemPurAmt)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Pur_Amt");
            entity.Property(e => e.ItemPurQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Pur_Qty");
            entity.Property(e => e.ItemScanCode)
                .HasMaxLength(50)
                .HasColumnName("Item_ScanCode");
            entity.Property(e => e.ItemSp)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Sp");
            entity.Property(e => e.ItemStkIdCat)
                .HasMaxLength(10)
                .HasColumnName("Item_StkId_Cat");
            entity.Property(e => e.ItemStkIdDesc).HasColumnName("Item_StkId_Desc");
            entity.Property(e => e.ItemStkIdGrp)
                .HasMaxLength(10)
                .HasColumnName("Item_StkId_Grp");
            entity.Property(e => e.ItemStkIdUnit)
                .HasMaxLength(5)
                .HasColumnName("Item_StkId_Unit");
            entity.Property(e => e.ItemSuppCode)
                .HasMaxLength(8)
                .HasColumnName("Item_SuppCode");
            entity.Property(e => e.ItemTrInAmt)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_TrIn_Amt");
            entity.Property(e => e.ItemTrInQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_TrIn_Qty");
            entity.Property(e => e.ItemTrOutAmt)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_TrOut_Amt");
            entity.Property(e => e.ItemTrOutQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_TrOut_Qty");
            entity.Property(e => e.ItemUp)
                .HasDefaultValue(0.00m)
                .HasComment("Cost Price at the time of Opening.")
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Up");

            entity.HasOne(d => d.Item).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Stock_Item_Master");
        });

        modelBuilder.Entity<StockCheck>(entity =>
        {
            entity.ToTable("StockCheck");

            entity.Property(e => e.RdExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Rd_ExpiryDate");
            entity.Property(e => e.RdListNo)
                .HasMaxLength(10)
                .HasColumnName("Rd_ListNo");
            entity.Property(e => e.RdLotNo)
                .HasMaxLength(10)
                .HasColumnName("Rd_LotNo");
            entity.Property(e => e.RdQty).HasColumnType("numeric(11, 2)");
            entity.Property(e => e.RdScanCode)
                .HasMaxLength(50)
                .HasColumnName("Rd_ScanCode");
            entity.Property(e => e.RdStkId).HasColumnName("Rd_StkId");
        });

        modelBuilder.Entity<StockForJournal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("StockForJournal");

            entity.Property(e => e.BalQty).HasColumnType("numeric(14, 2)");
            entity.Property(e => e.ClientName)
                .HasMaxLength(150)
                .HasColumnName("Client_Name");
            entity.Property(e => e.ItemBatchId).HasColumnName("Item_BatchId");
            entity.Property(e => e.ItemDesc)
                .HasMaxLength(300)
                .HasColumnName("Item_Desc");
            entity.Property(e => e.ItemExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Item_ExpiryDate");
            entity.Property(e => e.ItemListNo)
                .HasMaxLength(10)
                .HasColumnName("Item_ListNo");
            entity.Property(e => e.ItemLotNo)
                .HasMaxLength(10)
                .HasColumnName("Item_LotNo");
            entity.Property(e => e.StkId).HasColumnName("Stk_Id");
        });

        modelBuilder.Entity<StockTran>(entity =>
        {
            entity.HasKey(e => e.StkId);

            entity.ToTable("Stock_Trans");

            entity.Property(e => e.StkId)
                .ValueGeneratedNever()
                .HasColumnName("Stk_Id");
            entity.Property(e => e.ItemBatchId).HasColumnName("Item_BatchId");
            entity.Property(e => e.ItemClientCode)
                .HasMaxLength(8)
                .HasColumnName("Item_ClientCode");
            entity.Property(e => e.ItemDelAmt)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Del_Amt");
            entity.Property(e => e.ItemDelQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Del_Qty");
            entity.Property(e => e.ItemExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Item_ExpiryDate");
            entity.Property(e => e.ItemId).HasColumnName("Item_Id");
            entity.Property(e => e.ItemListNo)
                .HasMaxLength(10)
                .HasColumnName("Item_ListNo");
            entity.Property(e => e.ItemLotNo)
                .HasMaxLength(10)
                .HasColumnName("Item_LotNo");
            entity.Property(e => e.ItemOpQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_OpQty");
            entity.Property(e => e.ItemPurAmt)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Pur_Amt");
            entity.Property(e => e.ItemPurQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Pur_Qty");
            entity.Property(e => e.ItemStkIdCat)
                .HasMaxLength(10)
                .HasColumnName("Item_StkId_Cat");
            entity.Property(e => e.ItemStkIdDesc).HasColumnName("Item_StkId_Desc");
            entity.Property(e => e.ItemStkIdGrp)
                .HasMaxLength(10)
                .HasColumnName("Item_StkId_Grp");
            entity.Property(e => e.ItemStkIdUnit)
                .HasMaxLength(5)
                .HasColumnName("Item_StkId_Unit");
            entity.Property(e => e.ItemSuppCode)
                .HasMaxLength(8)
                .HasColumnName("Item_SuppCode");
            entity.Property(e => e.ItemTrInAmt)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_TrIn_Amt");
            entity.Property(e => e.ItemTrInQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_TrIn_Qty");
            entity.Property(e => e.ItemTrOpAmt)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_TrOpAmt");
            entity.Property(e => e.ItemTrOpQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_TrOpQty");
            entity.Property(e => e.ItemTrOutAmt)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_TrOut_Amt");
            entity.Property(e => e.ItemTrOutQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_TrOut_Qty");
            entity.Property(e => e.ItemUp)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Up");
			entity.Property(e => e.ItemCp)
            	.HasDefaultValue(0.00m)
            	.HasColumnType("numeric(11, 2)")
	            .HasColumnName("Item_Cp");

			entity.HasOne(d => d.Item).WithMany(p => p.StockTrans)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK1_Stock_Item_Master");
        });

        modelBuilder.Entity<SupplierMaster>(entity =>
        {
            entity.HasKey(e => e.SuppId);

            entity.ToTable("Supplier_Master");

            entity.HasIndex(e => e.SuppCode, "IX_Supplier_Master").IsUnique();

            entity.Property(e => e.SuppId).HasColumnName("Supp_Id");
            entity.Property(e => e.SuppAddr)
                .HasMaxLength(300)
                .HasColumnName("Supp_Addr");
            entity.Property(e => e.SuppCity)
                .HasMaxLength(5)
                .HasColumnName("Supp_City");
            entity.Property(e => e.SuppCode)
                .HasMaxLength(8)
                .HasColumnName("Supp_Code");
            entity.Property(e => e.SuppContPerson)
                .HasMaxLength(150)
                .HasColumnName("Supp_Cont_Person");
            entity.Property(e => e.SuppCountry)
                .HasMaxLength(5)
                .HasColumnName("Supp_Country");
            entity.Property(e => e.SuppCrNo)
                .HasMaxLength(20)
                .HasColumnName("Supp_CR_No");
            entity.Property(e => e.SuppEMail)
                .HasMaxLength(100)
                .HasColumnName("Supp_eMail");
            entity.Property(e => e.SuppZohoVendId)
                .HasMaxLength(25)
                .HasColumnName("Supp_Zoho_Vend_Id");
            entity.Property(e => e.SuppName)
                .HasMaxLength(150)
                .HasColumnName("Supp_Name");
            entity.Property(e => e.SuppPhone)
                .HasMaxLength(60)
                .HasColumnName("Supp_Phone");
            entity.Property(e => e.SuppRemarks).HasColumnName("Supp_Remarks");
            entity.Property(e => e.SuppUrl)
                .HasMaxLength(100)
                .HasColumnName("Supp_URL");

            entity.HasOne(d => d.GenCity).WithMany(p => p.SupplierMasters)
                .HasPrincipalKey(p => new { p.CityCountryCode, p.CityCode })
                .HasForeignKey(d => new { d.SuppCountry, d.SuppCity })
                .HasConstraintName("FK_Supplier_Master_Gen_City");
        });

        modelBuilder.Entity<SysPagesControl>(entity =>
        {
            entity.HasKey(e => e.SysPagesId);

            entity.ToTable("SysPagesControl");

            entity.Property(e => e.SysPagesId).HasColumnName("SysPages_Id");
            entity.Property(e => e.SysPagesAuthorized).HasColumnName("SysPages_Authorized");
            entity.Property(e => e.SysPagesControlId).HasColumnName("SysPages_Control_Id");
            entity.Property(e => e.SysPagesEmail)
                .HasMaxLength(30)
                .HasColumnName("SysPages_Email");

            entity.HasOne(d => d.SysPagesEmailNavigation).WithMany(p => p.SysPagesControls)
                .HasPrincipalKey(p => p.Email)
                .HasForeignKey(d => d.SysPagesEmail)
                .HasConstraintName("FK_SysPagesControl_AdminInfo");
        });

        modelBuilder.Entity<SystemPage>(entity =>
        {
            entity.HasKey(e => e.PageId);

            entity.Property(e => e.PageId)
                .ValueGeneratedNever()
                .HasColumnName("Page_Id");
            entity.Property(e => e.PageCompType)
                .HasMaxLength(10)
                .HasColumnName("Page_CompType");
            entity.Property(e => e.PageDetail)
                .HasMaxLength(200)
                .HasColumnName("Page_Detail");
            entity.Property(e => e.PageUrl)
                .HasMaxLength(50)
                .HasColumnName("Page_Url");
        });

        modelBuilder.Entity<TrDetail>(entity =>
        {
            entity.HasKey(e => e.TrdId);

            entity.ToTable("Tr_Detail");

            entity.Property(e => e.TrdId).HasColumnName("Trd_Id");
            entity.Property(e => e.TrdBatchId).HasColumnName("Trd_BatchId");
            entity.Property(e => e.TrdAlert)
                .HasDefaultValue(false)
                .HasColumnName("Trd_Alert");
            entity.Property(e => e.TrdAction)
                .HasDefaultValue(false)
                .HasColumnName("Trd_Action");
            entity.Property(e => e.TrdHighlightPriceDiff)
                .HasDefaultValue(false)
                .HasColumnName("Trd_Highlight_PriceDiff");

            entity.Property(e => e.TrdAlertStop)
                .HasDefaultValue(false)
                .HasColumnName("Trd_Alert_Stop");

            entity.Property(e => e.TrdBalJournQty)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Trd_Bal_Journ_Qty");
            entity.Property(e => e.TrdClientCodeFrom)
                .HasMaxLength(8)
                .HasColumnName("Trd_ClientCode_From");
            entity.Property(e => e.TrdClientCodeFromQty)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Trd_ClientCode_From_Qty");
            entity.Property(e => e.TrdClientCodeFromUp)
                .HasDefaultValue(0.00m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Trd_ClientCode_From_Up");
            entity.Property(e => e.TrdClientCodeTo)
                .HasMaxLength(8)
                .HasColumnName("Trd_ClientCode_To");
            entity.Property(e => e.TrdClientCodeToUp)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Trd_ClientCode_To_Up");
            entity.Property(e => e.TrdExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Trd_ExpiryDate");
            entity.Property(e => e.TrdIdRevJourn).HasColumnName("TrdId_RevJourn");
            entity.Property(e => e.TrdRevSeq).HasColumnName("Trd_RevSeq");
            entity.Property(e => e.TrdListNo)
                .HasMaxLength(10)
                .HasColumnName("Trd_ListNo");
            entity.Property(e => e.TrdLotNo)
                .HasMaxLength(10)
                .HasColumnName("Trd_LotNo");
            entity.Property(e => e.TrdStkIdDesc).HasColumnName("Trd_StkId_Desc");
            entity.Property(e => e.TrdStockStkId).HasColumnName("Trd_Stock_StkId");
            entity.Property(e => e.TrdTrhId).HasColumnName("Trd_TrhId");

            entity.Property(e => e.TrdLotChange)
                 .HasMaxLength(10)
                 .HasColumnName("Trd_LotChange");
            entity.Property(e => e.TrdReversal)
                 .HasMaxLength(10)
                 .HasColumnName("Trd_Reversal");

            entity.HasOne(d => d.TrdTrh).WithMany(p => p.TrDetails)
                .HasForeignKey(d => d.TrdTrhId)
                .HasConstraintName("FK_Tr_Detail_Tr_Head");
            entity.Property(e => e.TrdRev).HasColumnName("Trd_Rev");
        });

        modelBuilder.Entity<TrHead>(entity =>
        {
            entity.HasKey(e => e.TrhId);

            entity.ToTable("Tr_Head");

            entity.HasIndex(e => e.TrhNo, "IX_Tr_Head").IsUnique();

            entity.Property(e => e.TrhId).HasColumnName("Trh_Id");
            entity.Property(e => e.TrhApproved).HasColumnName("Trh_Approved");
            entity.Property(e => e.TrhExcludeAlertAction).HasColumnName("Trh_ExcludeAlertAction");
            entity.Property(e => e.TrhDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Trh_Date");
            entity.Property(e => e.TrhDateAltered)
                .HasColumnType("smalldatetime")
                .HasColumnName("Trh_Date_Altered");
            entity.Property(e => e.TrhBranch)
                .HasMaxLength(3)
                .HasColumnName("Trh_Branch");
            entity.Property(e => e.TrhComp)
                .HasMaxLength(2)
                .HasColumnName("Trh_Comp");
            entity.Property(e => e.TrhDispNo)
                .HasMaxLength(20)
                .HasColumnName("Trh_DispNo");
            entity.Property(e => e.TrhNo).HasColumnName("Trh_No");
            entity.Property(e => e.TrhRemarks)
                .IsUnicode(false)
                .HasColumnName("Trh_Remarks");
            entity.Property(e => e.TrhUser)
                .HasMaxLength(30)
                .HasColumnName("Trh_User");
        });

        modelBuilder.Entity<VwBalQty>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_BalQty");

            entity.Property(e => e.BalQty).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.ItemClientCode)
                .HasMaxLength(8)
                .HasColumnName("Item_ClientCode");
            entity.Property(e => e.ItemExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Item_ExpiryDate");
            entity.Property(e => e.ItemListNo)
                .HasMaxLength(10)
                .HasColumnName("Item_ListNo");
            entity.Property(e => e.ItemLotNo)
                .HasMaxLength(10)
                .HasColumnName("Item_LotNo");
        });

        modelBuilder.Entity<VwItemBal>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwItemBal");

            entity.Property(e => e.BalQty).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.ItemClientCode)
                .HasMaxLength(8)
                .HasColumnName("Item_ClientCode");
            entity.Property(e => e.ItemExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Item_ExpiryDate");
            entity.Property(e => e.StkId).HasColumnName("Stk_Id");
            entity.Property(e => e.ItemListNo)
                .HasMaxLength(10)
                .HasColumnName("Item_ListNo");
            entity.Property(e => e.ItemLotNo)
                .HasMaxLength(10)
                .HasColumnName("Item_LotNo");
        });

        modelBuilder.Entity<VwReceipt>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwReceipts");

            entity.Property(e => e.CatDesc)
                .HasMaxLength(30)
                .HasColumnName("Cat_Desc");
            entity.Property(e => e.ClientName)
                .HasMaxLength(150)
                .HasColumnName("Client_Name");
            entity.Property(e => e.GrpDesc)
                .HasMaxLength(30)
                .HasColumnName("Grp_Desc");
            entity.Property(e => e.ItemDesc)
                .HasMaxLength(300)
                .HasColumnName("Item_Desc");
            entity.Property(e => e.ItemGrpCode)
                .HasMaxLength(10)
                .HasColumnName("Item_GrpCode");
            entity.Property(e => e.ItemUnit)
                .HasMaxLength(5)
                .HasColumnName("Item_Unit");
            entity.Property(e => e.RdClientCode)
                .HasMaxLength(8)
                .HasColumnName("Rd_ClientCode");
            entity.Property(e => e.RdExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Rd_ExpiryDate");
            entity.Property(e => e.RdListNo)
                .HasMaxLength(10)
                .HasColumnName("Rd_ListNo");
            entity.Property(e => e.RdLotNo)
                .HasMaxLength(10)
                .HasColumnName("Rd_LotNo");
            entity.Property(e => e.RdStkIdGrp)
                .HasMaxLength(10)
                .HasColumnName("Rd_StkId_Grp");
            entity.Property(e => e.RdQty)
                .HasColumnType("numeric(38, 2)")
                .HasColumnName("Rd_Qty");
            entity.Property(e => e.RdStkId).HasColumnName("Rd_StkId");
            entity.Property(e => e.RdTotal).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.RdUp)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Rd_Up");
            entity.Property(e => e.RdVendInvDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Rd_Vend_InvDate");
            entity.Property(e => e.RdVendInvNo)
                .HasMaxLength(20)
                .HasColumnName("Rd_Vend_InvNo");
            entity.Property(e => e.RhApproved).HasColumnName("Rh_Approved");
            entity.Property(e => e.RhDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Rh_Date");
            entity.Property(e => e.RhDispNo)
                .HasMaxLength(20)
                .HasColumnName("Rh_Disp_No");
            entity.Property(e => e.SuppName)
                .HasMaxLength(150)
                .HasColumnName("Supp_Name");
            entity.Property(e => e.RdStockStkId)
            .HasColumnName("Rd_Stock_StkId");
        });


		modelBuilder.Entity<VwSale>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwSales");

            entity.Property(e => e.CatDesc)
                .HasMaxLength(30)
                .HasColumnName("Cat_Desc");
            entity.Property(e => e.CityName)
                .HasMaxLength(50)
                .HasColumnName("City_Name");
            entity.Property(e => e.ClientId).HasColumnName("Client_Id");
            entity.Property(e => e.ClientName)
                .HasMaxLength(150)
                .HasColumnName("Client_Name");
            entity.Property(e => e.ClientVendCode)
                .HasMaxLength(10)
                .HasColumnName("Client_Vend_Code");
            entity.Property(e => e.DelApproved).HasColumnName("Del_Approved");
            entity.Property(e => e.DelClientCode)
                .HasMaxLength(8)
                .HasColumnName("Del_ClientCode");
            entity.Property(e => e.DelDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Del_Date");
            entity.Property(e => e.DelDispNo)
                .HasMaxLength(20)
                .HasColumnName("Del_Disp_No");
            entity.Property(e => e.DelExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Del_ExpiryDate");
            entity.Property(e => e.DelStkIdGrp)
                .HasMaxLength(10)
                .HasColumnName("Del_StkId_Grp");
            entity.Property(e => e.DelBatchId).HasColumnName("Del_BatchId");
            entity.Property(e => e.DelListNo)
                .HasMaxLength(10)
                .HasColumnName("Del_ListNo");
            entity.Property(e => e.DelLotNo)
                .HasMaxLength(10)
                .HasColumnName("Del_LotNo");
            entity.Property(e => e.DelPurchPrice)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Del_PurchPrice");
            entity.Property(e => e.DelQty)
                .HasColumnType("numeric(38, 2)")
                .HasColumnName("Del_Qty");
            entity.Property(e => e.DelTotal).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.DelUprice)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Del_Uprice");
            entity.Property(e => e.Gross).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.GrpDesc)
                .HasMaxLength(30)
                .HasColumnName("Grp_Desc");
            entity.Property(e => e.ItemCatCode)
                .HasMaxLength(10)
                .HasColumnName("Item_CatCode");
            entity.Property(e => e.ItemDesc)
                .HasMaxLength(300)
                .HasColumnName("Item_Desc");
            entity.Property(e => e.ItemGrpCode)
                .HasMaxLength(10)
                .HasColumnName("Item_GrpCode");
            entity.Property(e => e.ItemId).HasColumnName("Item_Id");
            entity.Property(e => e.ItemProdCode)
                .HasMaxLength(50)
                .HasColumnName("Item_ProdCode");
            entity.Property(e => e.ItemUnit)
                .HasMaxLength(5)
                .HasColumnName("Item_Unit");
            entity.Property(e => e.Margin).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.PurchTotal).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.DelStockStkId)
            .HasColumnName("Del_Stock_StkId");
        });

        modelBuilder.Entity<VwStockForPriceUpd>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_StockForPriceUpd");

            entity.Property(e => e.ClientName)
                .HasMaxLength(150)
                .HasColumnName("Client_Name");
            entity.Property(e => e.ItemBatchId).HasColumnName("Item_BatchId");
            entity.Property(e => e.ItemClientCode)
                .HasMaxLength(8)
                .HasColumnName("Item_ClientCode");
            entity.Property(e => e.ItemCp)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Cp");
            entity.Property(e => e.ItemDesc)
                .HasMaxLength(300)
                .HasColumnName("Item_Desc");
            entity.Property(e => e.ItemListNo)
                .HasMaxLength(10)
                .HasColumnName("Item_ListNo");
        });

        modelBuilder.Entity<VwStockForSpriceUpd>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_StockForSpriceUpd");

            entity.Property(e => e.ClientName)
                .HasMaxLength(150)
                .HasColumnName("Client_Name");
            entity.Property(e => e.ItemBatchId).HasColumnName("Item_BatchId");
            entity.Property(e => e.ItemClientCode)
                .HasMaxLength(8)
                .HasColumnName("Item_ClientCode");
            entity.Property(e => e.ItemSp)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Item_Sp");
            entity.Property(e => e.ItemDesc)
                .HasMaxLength(300)
                .HasColumnName("Item_Desc");
            entity.Property(e => e.ItemListNo)
                .HasMaxLength(10)
                .HasColumnName("Item_ListNo");
        });

        modelBuilder.Entity<VwTransfer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwTransfers");

            entity.Property(e => e.AlertAmt).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.AmtFrom).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.CatDesc)
                .HasMaxLength(30)
                .HasColumnName("Cat_Desc");
            entity.Property(e => e.ClientFrom).HasMaxLength(150);
            entity.Property(e => e.ClientTo).HasMaxLength(150);
            entity.Property(e => e.DiffAmt).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.DiffPurchPrice).HasColumnType("numeric(12, 4)");
            entity.Property(e => e.GrpDesc)
                .HasMaxLength(30)
                .HasColumnName("Grp_Desc");
			entity.Property(e => e.ItemGrpCode)
	            .HasMaxLength(10)
	            .HasColumnName("Item_GrpCode");
			entity.Property(e => e.RevAmt).HasColumnType("numeric(38, 6)");

            entity.Property(e => e.ItemDesc)
                .HasMaxLength(300)
                .HasColumnName("Item_Desc");
            entity.Property(e => e.ItemUnit)
                .HasMaxLength(5)
                .HasColumnName("Item_Unit");
            entity.Property(e => e.NonAlertAmt).HasColumnType("numeric(38, 6)");
            entity.Property(e => e.OrderQty).HasColumnType("numeric(11, 2)");
            entity.Property(e => e.QtyFrom).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.TrdAlert).HasColumnName("Trd_Alert");
            entity.Property(e => e.TrdAction).HasColumnName("Trd_Action");
            entity.Property(e => e.TrdHighlightPriceDiff).HasColumnName("Trd_Highlight_PriceDiff");
            entity.Property(e => e.TrdAlertStop).HasColumnName("Trd_Alert_Stop");
            entity.Property(e => e.TrdBalJournQty)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Trd_Bal_Journ_Qty");
            entity.Property(e => e.TrdClientCodeFrom)
                .HasMaxLength(8)
                .HasColumnName("Trd_ClientCode_From");
            entity.Property(e => e.TrdBatchId).HasColumnName("Trd_BatchId");
            entity.Property(e => e.TrdClientCodeFromQty)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Trd_ClientCode_From_Qty");
            entity.Property(e => e.TrdClientCodeFromUp)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Trd_ClientCode_From_Up");
            entity.Property(e => e.TrdClientCodeTo)
                .HasMaxLength(8)
                .HasColumnName("Trd_ClientCode_To");
            entity.Property(e => e.TrdClientCodeToUp)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Trd_ClientCode_To_Up");
            entity.Property(e => e.TrdExpiryDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Trd_ExpiryDate");
            entity.Property(e => e.TrdId).HasColumnName("Trd_Id");
            entity.Property(e => e.TrdIdRevJourn).HasColumnName("TrdId_RevJourn");
            entity.Property(e => e.TrdRevSeq).HasColumnName("Trd_RevSeq");
            entity.Property(e => e.TrdListNo)
                .HasMaxLength(10)
                .HasColumnName("Trd_ListNo");
            entity.Property(e => e.TrdLotNo)
                .HasMaxLength(10)
                .HasColumnName("Trd_LotNo");
            entity.Property(e => e.TrdStkIdDesc).HasColumnName("Trd_StkId_Desc");
            entity.Property(e => e.TrdStockStkId).HasColumnName("Trd_Stock_StkId");
            entity.Property(e => e.TrhApproved).HasColumnName("Trh_Approved");
            entity.Property(e => e.TrhDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Trh_Date");
            entity.Property(e => e.TrhDispNo)
                .HasMaxLength(20)
                .HasColumnName("Trh_DispNo");

            entity.Property(e => e.TrdLotChange)
                .HasMaxLength(10)
                .HasColumnName("Trd_LotChange");
            entity.Property(e => e.TrdReversal)
                .HasMaxLength(10)
                .HasColumnName("Trd_Reversal");
            entity.Property(e => e.TrdRev).HasColumnName("Trd_Rev");
        });

        modelBuilder.Entity<VwPurchaseOrder>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwPurchaseOrders");

            entity.Property(e => e.ClientName)
                .HasMaxLength(150)
                .HasColumnName("Client_Name");
            entity.Property(e => e.ClientCode)
                .HasMaxLength(8)
                .HasColumnName("Client_Code");
            entity.Property(e => e.ItemDesc)
                .HasMaxLength(300)
                .HasColumnName("Item_Desc");
            entity.Property(e => e.ItemUnit)
                .HasMaxLength(5)
                .HasColumnName("Item_Unit");
            entity.Property(e => e.PoQty).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.PoRcvdQty).HasColumnType("numeric(38, 2)");
            entity.Property(e => e.PoRcvdTotal).HasColumnType("numeric(38, 4)");
            entity.Property(e => e.PoTotal).HasColumnType("numeric(38, 4)");
            entity.Property(e => e.PodListNo)
                .HasMaxLength(10)
                .HasColumnName("Pod_ListNo");
            entity.Property(e => e.PodUp)
                .HasColumnType("numeric(11, 2)")
                .HasColumnName("Pod_Up");
            entity.Property(e => e.PohApproved).HasColumnName("Poh_Approved");
            entity.Property(e => e.PohConvRate)
                .HasColumnType("numeric(8, 4)")
                .HasColumnName("Poh_ConvRate");
            entity.Property(e => e.PohCurr)
                .HasMaxLength(5)
                .HasColumnName("Poh_Curr");
            entity.Property(e => e.PohDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("Poh_Date");
            entity.Property(e => e.PohDispNo)
                .HasMaxLength(20)
                .HasColumnName("Poh_Disp_No");
            entity.Property(e => e.SuppName)
                .HasMaxLength(150)
                .HasColumnName("Supp_Name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
