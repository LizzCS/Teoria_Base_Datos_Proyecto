USE sistema_bancario;
GO

CREATE OR ALTER PROCEDURE sp_reporte_distribucion_gastos
    @p_anio smallint,
    @p_mes tinyint
    AS
    BEGIN
        SELECT 
            c.id_categoria, 
            c.nombre,
            ISNULL(SUM(t.monto), 0) AS total,
            COUNT(t.id_transaccion)  AS num_transacciones
        FROM categoria c
        INNER JOIN subcategoria s         
            ON s.id_categoria = c.id_categoria
        INNER JOIN presupuesto_detalle pd  
            ON pd.id_subcategoria = s.id_subcategoria
        INNER JOIN transaccion t 
            ON t.id_presupuesto_detalle = pd.id
        WHERE c.tipo_categoria   = 'gasto'
          AND t.anio_transaccion = @p_anio
          AND t.mes_transaccion  = @p_mes
        GROUP BY c.id_categoria, c.nombre
        ORDER BY 3 DESC
    END
GO

CREATE OR ALTER PROCEDURE sp_reporte_cumplimiento_presupuesto
    @p_id_presupuesto int,
    @p_anio smallint,
    @p_mes tinyint
AS
BEGIN
    SELECT s.nombre,
    pd.monto_mensual,
    ISNULL(SUM(t.monto), 0) AS ejecutado
    FROM presupuesto_detalle pd
        INNER JOIN subcategoria s 
            ON s.id_subcategoria = pd.id_subcategoria
        LEFT JOIN transaccion  t 
            ON t.id_presupuesto_detalle = pd.id
            AND t.anio_transaccion = @p_anio
            AND t.mes_transaccion  = @p_mes
    WHERE pd.id_presupuesto = @p_id_presupuesto
    GROUP BY s.nombre, pd.monto_mensual
    ORDER BY s.nombre
    END
GO

CREATE OR ALTER PROCEDURE sp_reporte_categorias_gasto
    AS
    BEGIN
        SELECT id_categoria, nombre
        FROM categoria
        WHERE tipo_categoria = 'gasto'
        ORDER BY nombre
    END
GO

CREATE OR ALTER PROCEDURE sp_reporte_estado_obligaciones
    @p_anio smallint,
    @p_mes tinyint
AS
BEGIN
    WITH pagos AS(
        SELECT 
            ot.id_obligacion,
            t.fecha_hora_registro,
            ROW_NUMBER() OVER (PARTITION BY ot.id_obligacion ORDER BY t.fecha_hora_registro DESC) AS rn
        FROM obligacion_transaccion ot
        INNER JOIN transaccion t
            ON t.id_transaccion = ot.id_transaccion
        WHERE t.anio_transaccion = @p_anio
          AND t.mes_transaccion  = @p_mes)
    SELECT 
        o.id_obligacion,
        o.nombre,
        o.monto_fijo_mensual,
        o.dia_del_mes,
        p.fecha_hora_registro AS ultimo_pago
    FROM obligacion_fija o
    LEFT JOIN pagos p
        ON p.id_obligacion = o.id_obligacion
       AND p.rn = 1
    WHERE o.esta_vigente = 1
    ORDER BY o.dia_del_mes;
END
GO

CREATE OR ALTER PROCEDURE sp_reporte_progreso_ahorros
    @p_id_presupuesto int,
    @p_anio smallint,
    @p_mes tinyint
    AS
    BEGIN
        SELECT 
            s.nombre,
            pd.monto_mensual,
            ISNULL(SUM(t.monto), 0) AS ejecutado
        FROM presupuesto_detalle pd
        INNER JOIN subcategoria s 
            ON s.id_subcategoria = pd.id_subcategoria
        INNER JOIN categoria c 
            ON c.id_categoria = s.id_categoria
        LEFT  JOIN transaccion t 
            ON t.id_presupuesto_detalle = pd.id
            AND t.anio_transaccion = @p_anio
            AND t.mes_transaccion  = @p_mes
        WHERE pd.id_presupuesto = @p_id_presupuesto
          AND c.tipo_categoria  = 'ahorro'
        GROUP BY s.nombre, pd.monto_mensual
        ORDER BY s.nombre
    END
GO