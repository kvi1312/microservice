
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if exists (select * from sysobjects where id= OBJECT_ID(N'[Create_Permissions]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
	drop procedure [Create_Permissions]
end
GO

CREATE PROCEDURE Create_Permissions 
	@roleId varchar(50) null,
	@function varchar(50) null,
	@command varchar(50) null,
	@newID bigint output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set xact_abort on;
	begin tran
	begin try

	if not exists (select * from [IdentityServer].[Identity].[Permissions] where RoleId = @roleId and [Function] = @function and Command = @command)
	Begin
		Insert into [IdentityServer].[Identity].[Permissions]([RoleId], [Function], [Command] )
		values (@roleId, @function, @command)
		set @newID = SCOPE_IDENTITY()
	End

	Commit
	End try
	Begin catch
		Rollback
			Declare @ErrorMessage varchar(200)
			Select @ErrorMessage = 'ERROR' + ERROR_MESSAGE()
			RAISERROR(@ErrorMessage, 16, 1)
	end catch
END
GO
