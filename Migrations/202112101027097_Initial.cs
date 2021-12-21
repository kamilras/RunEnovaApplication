namespace RunEnovaApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bazas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NazwaBazy = c.String(),
                        Operator = c.String(),
                        FolderApp = c.String(),
                        FolderUIApp = c.String(),
                        BezDLLSerweraApp = c.Boolean(nullable: false),
                        BezDodatkowApp = c.Boolean(nullable: false),
                        ListaBazDanychApp = c.String(),
                        FolderDodatkowApp = c.String(),
                        KonfiguracjaApp = c.String(),
                        FolderServ = c.String(),
                        BezHarmonogramuServ = c.Boolean(nullable: false),
                        BezDLLSerweraServ = c.Boolean(nullable: false),
                        BezDodatkowServ = c.Boolean(nullable: false),
                        PortServ = c.String(),
                        ListaBazDanychServ = c.String(),
                        FolderDodatkowServ = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Bazas");
        }
    }
}
