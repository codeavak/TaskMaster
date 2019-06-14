namespace TaskMaster.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedStatusClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            Sql("insert into status(name) values ('To Do')");
            Sql("insert into status(name) values ('In Progress')");
            Sql("insert into status(name) values ('Done')");
        }
        
        public override void Down()
        {
            DropTable("dbo.Status");
        }
    }
}
