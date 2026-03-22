ALTER TABLE transaccion
ADD descripcion VARCHAR(300)
GO

ALTER TABLE transaccion
ADD metodo_pago VARCHAR (300)
GO

ALTER TABLE transaccion
DROP COLUMN numero_factura
GO