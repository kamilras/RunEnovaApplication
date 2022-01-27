namespace RunEnovaApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Zmiany : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Bazas", newName: "Baza");
            AddColumn("dbo.Baza", "NazwaBazySQL", c => c.String());
            AddColumn("dbo.Baza", "NazwaBazyEnova", c => c.String());
            DropColumn("dbo.Baza", "NazwaBazy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Baza", "NazwaBazy", c => c.String());
            DropColumn("dbo.Baza", "NazwaBazyEnova");
            DropColumn("dbo.Baza", "NazwaBazySQL");
            RenameTable(name: "dbo.Baza", newName: "Bazas");
        }
    }
}
