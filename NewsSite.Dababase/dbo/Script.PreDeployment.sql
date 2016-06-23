-- This script will clean the Database recreating three users with their roles
-- Main DB is located into NewsSite.WebApplication's project structure, under the App_start folder
-- DB is copied to Test Project's local folder automatically during build, so that tests cannot impact the working copy
-- Path is: $(SolutionPath)\NewsSite.WebApplication\App_Data\aspnet-NewsSite.WebApplication-20160612113930.mdf

-- Cleans DB
DELETE FROM [Like]
DELETE FROM [Articles]
DELETE FROM [AspNetUserRoles]
DELETE FROM [AspNetUsers]
DELETE FROM [AspNetRoles]

-- Creates two roles
-- Employee, limited to read and like articles
-- Publisher, who can also create, edit and delete articles, and view some stats
INSERT INTO [AspNetRoles] ([Id],[Name]) VALUES ('b938a800-076f-42aa-aeb8-54fad97a3624','Employee')
INSERT INTO [AspNetRoles] ([Id],[Name]) VALUES ('96a509fa-7233-4312-a0d4-c15a7896b365','Publisher')

-- Creates three users
-- 1 Employee and 2 Publishers
INSERT INTO [AspNetUsers] VALUES ('fc9c5c06-814a-4c30-a541-a76ba3465b6e','London','test_employee@newssite.co.uk',0,'AMYnIA4EnbLVqvgEX7+dShc7hkAv4QCvI+nwP8LBKQQaEQqwqKndvnl6vNbnaQO0/A==','c176ec16-8277-4be4-886c-bb2d5ddeba93',NULL,0,0,NULL,1,0,'test_employee@newssite.co.uk')
INSERT INTO [AspNetUsers] VALUES ('a095d0a7-b38b-4827-97a9-36be9b91520d','London','test_publisher@newssite.co.uk',0,'AMWMlyyvq4qoydtP/wsuKbTdnrxKmEfWHlgSbGlvxAVMuk+B4OBDj3SVKlSV2j69qQ==','bab4e758-e486-4286-80e0-2864066b4ab4',NULL,0,0,NULL,1,0,'test_publisher@newssite.co.uk')
INSERT INTO [AspNetUsers] VALUES ('fd6a6985-ae73-45f4-83a2-ede321906fb1','London','test_publisher2@newssite.co.uk',0,'AL/12ULOq0h1f8xckr2ylvBlMWJS1uJ1MeJOKvjXiCjkIUhhPg2HIdvw2HSFtnd2hQ==','a1a2a678-f242-4b43-8ecf-edc8753f7c94',NULL,0,0,NULL,1,0,'test_publisher2@newssite.co.uk')

-- Adds roles to the users
INSERT INTO [AspNetUserRoles] VALUES ('fc9c5c06-814a-4c30-a541-a76ba3465b6e','b938a800-076f-42aa-aeb8-54fad97a3624')
INSERT INTO [AspNetUserRoles] VALUES ('a095d0a7-b38b-4827-97a9-36be9b91520d','96a509fa-7233-4312-a0d4-c15a7896b365')
INSERT INTO [AspNetUserRoles] VALUES ('fd6a6985-ae73-45f4-83a2-ede321906fb1','96a509fa-7233-4312-a0d4-c15a7896b365')