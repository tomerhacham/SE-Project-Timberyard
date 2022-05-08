USE [master]
GO

/****** Object:  Database [Timberyard]    Script Date: 11/04/2022 13:58:53 ******/
CREATE DATABASE [Timberyard]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Timberyard', FILENAME = N'/var/opt/mssql/data/Timberyard.mdf' , SIZE = 1056768KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Timberyard_log', FILENAME = N'/var/opt/mssql/data/Timberyard_log.ldf' , SIZE = 7413760KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Timberyard].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

/****** Object:  Login [etl_process]    Script Date: 11/04/2022 14:56:52 ******/
CREATE LOGIN [etl_process] WITH PASSWORD='etl_process', DEFAULT_DATABASE=[Timberyard], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

ALTER LOGIN [etl_process] ENABLE
GO

/****** Object:  Login [timberyard_service]    Script Date: 11/04/2022 14:56:52 ******/
CREATE LOGIN [timberyard_service] WITH PASSWORD='timberyard_service', DEFAULT_DATABASE=[Timberyard], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

ALTER LOGIN [timberyard_service] ENABLE
GO


ALTER DATABASE [Timberyard] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [Timberyard] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [Timberyard] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [Timberyard] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [Timberyard] SET ARITHABORT OFF 
GO

ALTER DATABASE [Timberyard] SET AUTO_CLOSE ON 
GO

ALTER DATABASE [Timberyard] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [Timberyard] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [Timberyard] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [Timberyard] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [Timberyard] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [Timberyard] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [Timberyard] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [Timberyard] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [Timberyard] SET  ENABLE_BROKER 
GO

ALTER DATABASE [Timberyard] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [Timberyard] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [Timberyard] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [Timberyard] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [Timberyard] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [Timberyard] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [Timberyard] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [Timberyard] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [Timberyard] SET  MULTI_USER 
GO

ALTER DATABASE [Timberyard] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [Timberyard] SET DB_CHAINING OFF 
GO

ALTER DATABASE [Timberyard] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [Timberyard] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [Timberyard] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [Timberyard] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [Timberyard] SET QUERY_STORE = OFF
GO

ALTER DATABASE [Timberyard] SET  READ_WRITE 
GO


USE [Timberyard]
GO
/****** Object:  User [etl_process]    Script Date: 11/04/2022 13:57:49 ******/
CREATE USER [etl_process] FOR LOGIN [etl_process] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [timberyard_service]    Script Date: 11/04/2022 13:57:49 ******/
CREATE USER [timberyard_service] FOR LOGIN [timberyard_service] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [etl_process]
GO
ALTER ROLE [db_owner] ADD MEMBER [timberyard_service]
GO


/****** Create table structure ******/

USE [Timberyard]
GO
/****** Object:  Table [dbo].[Alarms]    Script Date: 08/05/2022 20:22:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Alarms](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Objective] [varchar](255) NOT NULL,
	[Field] [int] NOT NULL,
	[Threshold] [int] NOT NULL,
	[Receivers] [text] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_Alarms] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Logs]    Script Date: 08/05/2022 20:22:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Logs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CardRev] [varchar](255) NOT NULL,
	[CardName] [varchar](255) NOT NULL,
	[SwRev] [varchar](255) NOT NULL,
	[DBRev] [varchar](255) NOT NULL,
	[Date] [date] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[NetTime] [time](7) NOT NULL,
	[SN] [varchar](255) NOT NULL,
	[Catalog] [varchar](255) NOT NULL,
	[Station] [varchar](255) NOT NULL,
	[Operator] [varchar](255) NOT NULL,
	[DBMode] [varchar](255) NOT NULL,
	[ContinueOnFail] [varchar](255) NOT NULL,
	[TECHMode] [varchar](255) NOT NULL,
	[ABORT] [varchar](255) NOT NULL,
	[FinalResult] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tests]    Script Date: 08/05/2022 20:22:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tests](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LogId] [int] NOT NULL,
	[Type] [varchar](255) NOT NULL,
	[Task] [varchar](255) NOT NULL,
	[Test] [varchar](255) NOT NULL,
	[Received] [text] NULL,
	[Expected] [varchar](255) NULL,
	[Max] [float] NULL,
	[Min] [float] NULL,
	[Result] [varchar](255) NOT NULL,
	[TestName] [varchar](255) NOT NULL,
	[DrationNet] [time](7) NOT NULL,
	[DurationGross] [time](7) NOT NULL,
 CONSTRAINT [PK_Tests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 08/05/2022 20:22:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Email] [varchar](255) NOT NULL,
	[Password] [varchar](255) NOT NULL,
	[Role] [int] NOT NULL,
	[ExperationTimeStamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Logs]  WITH CHECK ADD  CONSTRAINT [FK_Logs_Logs] FOREIGN KEY([Id])
REFERENCES [dbo].[Logs] ([Id])
GO
ALTER TABLE [dbo].[Logs] CHECK CONSTRAINT [FK_Logs_Logs]
GO
ALTER TABLE [dbo].[Tests]  WITH NOCHECK ADD  CONSTRAINT [FK_Tests_Logs] FOREIGN KEY([LogId])
REFERENCES [dbo].[Logs] ([Id])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[Tests] CHECK CONSTRAINT [FK_Tests_Logs]
GO
