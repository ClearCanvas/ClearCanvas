PRINT N'Deleting existing user sessions'
delete from UserSession_
GO

alter table UserSession_ add IsImpersonated_ BIT not null default 0
GO

PRINT N'Create AccountTypeEnum_ table'
create table UserAccountTypeEnum_ (Code_ NVARCHAR(255) not null, Value_ NVARCHAR(50) null, Description_ NVARCHAR(200) null, DisplayOrder_ REAL null, Deactivated_ BIT not null default 0, primary key (Code_))
GO

insert into UserAccountTypeEnum_ (Code_, Value_, Description_, DisplayOrder_, Deactivated_) values ('U', 'User', NULL, 1, 'False')
insert into UserAccountTypeEnum_ (Code_, Value_, Description_, DisplayOrder_, Deactivated_) values ('G', 'Group', NULL, 2, 'False')
insert into UserAccountTypeEnum_ (Code_, Value_, Description_, DisplayOrder_, Deactivated_) values ('S', 'Service', NULL, 3, 'False')
GO

PRINT N'Add AccountType_ field to the User_ table'
alter table User_ add AccountType_ NVARCHAR(255)
GO

PRINT N'Set all existing accounts to type U before adding NOT NULL constraint on column'
update User_ set AccountType_ = 'U'
alter table User_ alter column AccountType_ NVARCHAR(255) NOT NULL

alter table User_ add constraint FK_AccountType_6EFE8F739003A904F7ECB8C9135C3F5A foreign key (AccountType_) references UserAccountTypeEnum_
create index IX_AccountType_6EFE8F739003A904F7ECB8C9135C3F5A on User_ (AccountType_)
GO

PRINT N'Update AuthorityGroup_ table'
alter table AuthorityGroup_ add BuiltIn_ BIT 
GO

update AuthorityGroup_ set BuiltIn_ = 0
alter table AuthorityGroup_ alter column BuiltIn_ BIT NOT NULL



