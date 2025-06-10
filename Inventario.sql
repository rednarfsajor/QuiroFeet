USE [QuiroFeet]
GO

/****** Object:  Table [dbo].[Inventario]    Script Date: 9/6/2025 22:42:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Inventario](
	[id] [int] NOT NULL,
	[id_producto] [int] NOT NULL,
	[stock] [int] NULL,
	[publico] [int] NULL,
 CONSTRAINT [PK_Inventario] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Inventario]  WITH CHECK ADD  CONSTRAINT [FK_Inventario_Producto] FOREIGN KEY([id_producto])
REFERENCES [dbo].[Productos] ([id])
GO

ALTER TABLE [dbo].[Inventario] CHECK CONSTRAINT [FK_Inventario_Producto]
GO

