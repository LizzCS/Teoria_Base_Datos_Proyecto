
USE sistema_bancario;
GO

-- INSERTAR
CREATE OR ALTER PROCEDURE sp_insertar_presupuesto_detalle
    @p_id_presupuesto int,
    @p_id_subcategoria int,
    @p_monto_mensual numeric(12,2),
    @p_observaciones varchar(255),
    @p_creado_por int
AS
BEGIN

    IF NOT EXISTS (SELECT 1 FROM presupuesto WHERE presupuesto_id = @p_id_presupuesto)
        BEGIN
        RAISERROR('El presupuesto no existe.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM subcategoria WHERE id_subcategoria = @p_id_subcategoria)
    BEGIN
        RAISERROR('La subcategoría no existe.', 16, 1);
        RETURN;
    END

    IF (@p_monto_mensual < 0)
    BEGIN
        RAISERROR('El monto mensual no puede ser negativo.', 16, 1);
        RETURN;
    END

    INSERT INTO presupuesto_detalle
        (id_presupuesto, id_subcategoria, monto_mensual, observacion_monto, creado_por, creado_en)
    VALUES
        (@p_id_presupuesto, @p_id_subcategoria, @p_monto_mensual, @p_observaciones, @p_creado_por, GETDATE());
    END
GO

-- ACTUALIZAR
CREATE OR ALTER PROCEDURE sp_actualizar_presupuesto_detalle
    @p_id_detalle int,
    @p_monto_mensual numeric (12,2),
    @p_observaciones varchar(255),
    @p_modificado_por int
    AS
    BEGIN

    IF NOT EXISTS ( SELECT 1 FROM presupuesto_detalle WHERE id = @p_id_detalle)
        BEGIN
            RAISERROR('El presupuesto detalle no existe.', 16, 1);
            RETURN;
        END

    UPDATE presupuesto_detalle
    SET monto_mensual = @p_monto_mensual,
        observacion_monto = @p_observaciones,
        modificado_por = @p_modificado_por,
        modificado_en = GETDATE()
    WHERE id = @p_id_detalle
    END
GO


---- ELIMINAR
CREATE OR ALTER PROCEDURE sp_eliminar_presupuesto_detalle
    @p_id_detalle int
    AS
    BEGIN
    IF EXISTS (SELECT 1 FROM transaccion WHERE id_presupuesto_detalle = @p_id_detalle)
    BEGIN
        RAISERROR('No se puede eliminar el detalle porque tiene transacciones asociadas.', 16, 1);
        RETURN;
    END
    IF NOT EXISTS ( SELECT 1 FROM presupuesto_detalle WHERE id = @p_id_detalle)
        BEGIN
            RAISERROR('El presupuesto detalle no existe.', 16, 1);
            RETURN;
        END
    DELETE FROM presupuesto_detalle
    WHERE id = @p_id_detalle 
  END
GO


-- CONSULTAR 
CREATE OR ALTER PROCEDURE sp_consultar_presupuesto_detalle
    @p_id_detalle int
AS
BEGIN

    IF NOT EXISTS (
        SELECT 1
        FROM presupuesto_detalle
        WHERE id = @p_id_detalle
    )
    BEGIN
        RAISERROR('El detalle con ID %d no existe.',16,1,@p_id_detalle);
        RETURN;
    END

    SELECT 
        d.id,
        d.id_presupuesto,
        p.nombre_descriptivo AS presupuesto_nombre,
        d.id_subcategoria,
        s.nombre AS subcategoria_nombre,
        s.descripcion AS subcategoria_descripcion,
        c.id_categoria,
        c.nombre AS categoria_nombre,
        d.monto_mensual,
        d.observacion_monto,
        d.creado_por,
        d.modificado_por,
        d.creado_en,
        d.modificado_en
    FROM presupuesto_detalle d
    INNER JOIN subcategoria s 
        ON d.id_subcategoria = s.id_subcategoria
    INNER JOIN categoria c 
        ON s.id_categoria = c.id_categoria
    INNER JOIN presupuesto p 
        ON d.id_presupuesto = p.presupuesto_id
    WHERE d.id = @p_id_detalle;

END
GO

---- LISTAR 
CREATE OR ALTER PROCEDURE sp_listar_detalles_presupuesto
    @p_id_presupuesto int
AS
BEGIN

    IF NOT EXISTS (
        SELECT 1
        FROM presupuesto
        WHERE presupuesto_id = @p_id_presupuesto
    )
    BEGIN
        RAISERROR('El presupuesto con ID %d no existe.',16,1,@p_id_presupuesto);
        RETURN;
    END

    SELECT
        d.id,
        d.id_subcategoria,
        s.nombre AS subcategoria_nombre,
        s.descripcion AS subcategoria_descripcion,
        d.monto_mensual,
        d.observacion_monto,
        d.creado_por,
        d.modificado_por,
        d.creado_en,
        d.modificado_en
    FROM presupuesto_detalle d
    INNER JOIN subcategoria s 
        ON d.id_subcategoria = s.id_subcategoria
    WHERE d.id_presupuesto = @p_id_presupuesto
END
GO
