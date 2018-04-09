
CREATE TABLE dbo.ContactUs
(
	Id							INT primary key IDENTITY(1,1) NOT NULL,	
	Name						NATIONAL character varying(256) not NULL,
	Email						NATIONAL character varying(256) not NULL,	
	Message						NATIONAL character varying(max) ,
	Subject						NATIONAL character varying(256) NULL
);
