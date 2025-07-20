
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if exists (select * from sysobjects where id= OBJECT_ID(N'[Delete_Permissions]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
begin
	drop procedure Delete_Permissions
end
GO

CREATE PROCEDURE Delete_Permissions 
	@roleId varchar(50) null,
	@function varchar(50) null,
	@command varchar(50) null
AS
BEGIN
	Delete
	from [IdentityServer].[Identity].[Permissions] 
	where [RoleId] = @roleId
	and [Function] = @function
	and [Command] = @command
END
GO
