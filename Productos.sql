USE [QuiroFeet]
GO

/****** Object:  Table [dbo].[Productos]    Script Date: 9/6/2025 22:41:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Productos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](100) NOT NULL,
	[id_proveedor] [int] NOT NULL,
	[descripcion] [nvarchar](200) NULL,
	[precio] [decimal](20, 2) NULL,
	[activo] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Productos]  WITH CHECK ADD  CONSTRAINT [FK_Productos_Proveedores] FOREIGN KEY([id_proveedor])
REFERENCES [dbo].[Proveedores] ([id])
GO

ALTER TABLE [dbo].[Productos] CHECK CONSTRAINT [FK_Productos_Proveedores]
GO

