USE Sistema_bancario;
GO

CREATE PROCEDURE sp_insertar_presupuesto_detalle
    @p_id_presupuesto INT,
    @p_id_subcategoria INT,
    @p_monto_mensual NUMERIC(12,2),
    @p_observaciones VARCHAR(255)
AS
BEGIN
    INSERT INTO presupuesto_detalle (id_presupuesto, id_subcategoria, monto_mensual, observaciones)
    VALUES (@p_id_presupuesto, @p_id_subcategoria, @p_monto_mensual, @p_observaciones);
END
GO

CREATE PROCEDURE sp_actualizar_presupuesto_detalle
    @p_id_detalle INT,
    @p_monto_mensual NUMERIC(12,2),
    @p_observaciones VARCHAR(255)
AS
BEGIN
    UPDATE presupuesto_detalle
    SET monto_mensual = @p_monto_mensual,
        observaciones = @p_observaciones
    WHERE id_detalle = @p_id_detalle;
END
GO

CREATE PROCEDURE sp_eliminar_presupuesto_detalle
    @p_id_detalle INT
AS
BEGIN
    DELETE FROM presupuesto_detalle
    WHERE id_detalle = @p_id_detalle;
END
GO

CREATE PROCEDURE sp_consultar_presupuesto_detalle
    @p_id_detalle INT
AS
BEGIN
    SELECT *
    FROM presupuesto_detalle
    WHERE id_detalle = @p_id_detalle;
END
GO

CREATE PROCEDURE sp_listar_presupuesto_detalles
AS
BEGIN
    SELECT *
    FROM presupuesto_detalle
    ORDER BY id_presupuesto, id_subcategoria;
END
GO
