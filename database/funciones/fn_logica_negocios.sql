USE sistema_bancario;
GO

-- CALCULAR MONTO FN
CREATE OR ALTER FUNCTION fn_calcular_monto_ejecutado(@id_subcategoria int, @anio smallint, @mes tinyint)
	RETURNS decimal(12,2)
	AS
	BEGIN

    IF NOT EXISTS (SELECT 1 FROM subcategoria  WHERE id_subcategoria = @id_subcategoria)
        RETURN NULL;

    IF (@mes NOT BETWEEN 1 AND 12)
        RETURN NULL;
	
	DECLARE @total decimal(12,2);

	SELECT
		@total = SUM (t.monto)
	FROM subcategoria s
	INNER JOIN presupuesto_detalle pd
		ON pd.id_subcategoria = s.id_subcategoria
	INNER JOIN transaccion t
		ON t.id_presupuesto_detalle = pd.id
	WHERE s.id_subcategoria = @id_subcategoria
			AND t.anio_transaccion = @anio
			AND t.mes_transaccion = @mes
	RETURN @total
	END
GO

-- CALCULAR PORCENTAJE 
CREATE OR ALTER FUNCTION fn_calcular_porcentaje_ejecutado(@id_subcategoria int, @anio smallint, @mes tinyint, @id_presupuesto int)
RETURNS DECIMAL(12,2)
AS
BEGIN

    IF NOT EXISTS (SELECT 1 FROM subcategoria  WHERE id_subcategoria = @id_subcategoria)
        RETURN NULL;

    IF NOT EXISTS (SELECT 1  FROM presupuesto WHERE presupuesto_id = @id_presupuesto)
        RETURN NULL;

    IF (@mes NOT BETWEEN 1 AND 12)
        RETURN NULL;

    DECLARE @ejecutado DECIMAL(12,2);
    DECLARE @presupuestado DECIMAL(12,2);

    SELECT @ejecutado = dbo.fn_calcular_monto_ejecutado(@id_subcategoria, @anio, @mes);

    SELECT @presupuestado = pd.monto_mensual
    FROM presupuesto_detalle pd
    WHERE pd.id_presupuesto = @id_presupuesto
      AND pd.id_subcategoria = @id_subcategoria;

    RETURN CASE
        WHEN @presupuestado IS NULL OR @presupuestado = 0
        THEN 0
        ELSE (@ejecutado / @presupuestado) * 100
    END;

END
GO

-- CALCULAR BALANCE SUBCATEGORIA
CREATE OR ALTER FUNCTION fn_obtener_balance_subcategoria(@id_presupuesto int, @id_subcategoria int, @anio smallint, @mes tinyint)
RETURNS DECIMAL(12,2)
AS
BEGIN

    IF NOT EXISTS (SELECT 1 FROM subcategoria  WHERE id_subcategoria = @id_subcategoria)
        RETURN NULL;

    IF NOT EXISTS (SELECT 1  FROM presupuesto WHERE presupuesto_id = @id_presupuesto)
        RETURN NULL;

    IF (@mes NOT BETWEEN 1 AND 12)
        RETURN NULL;
    
    DECLARE @ejecutado decimal(12,2);
    DECLARE @presupuestado decimal(12,2);

    SELECT @ejecutado = dbo.fn_calcular_monto_ejecutado(@id_subcategoria,@anio,@mes);

    SELECT @presupuestado = pd.monto_mensual
    FROM presupuesto_detalle pd
    WHERE pd.id_presupuesto = @id_presupuesto
      AND pd.id_subcategoria = @id_subcategoria;

    RETURN ISNULL(@presupuestado,0) - ISNULL(@ejecutado,0);
    END
GO

-- OBTENER TOTAL DEL MES
CREATE OR ALTER FUNCTION fn_obtener_total_categoria_mes(@id_categoria INT, @id_presupuesto INT, @anio smallint, @mes TINYINT)
RETURNS decimal(12,2)
AS
BEGIN
    
    IF NOT EXISTS (SELECT 1 FROM categoria  WHERE id_categoria = @id_categoria)
        RETURN NULL;

    IF NOT EXISTS (SELECT 1  FROM presupuesto WHERE presupuesto_id = @id_presupuesto)
        RETURN NULL;

    IF (@mes NOT BETWEEN 1 AND 12)
        RETURN NULL;
  
    DECLARE @total decimal(12,2);

    SELECT @total = ISNULL(SUM(pd.monto_mensual),0)
    FROM subcategoria s
    INNER JOIN presupuesto_detalle pd
        ON pd.id_subcategoria = s.id_subcategoria
    WHERE s.id_categoria = @id_categoria
      AND pd.id_presupuesto = @id_presupuesto;
    RETURN @total;
    END
GO

CREATE OR ALTER FUNCTION fn_obtener_total_ejecutado_categoria_mes( @id_categoria INT, @anio smallint, @mes tinyint)
RETURNS decimal(12,2)
AS
BEGIN
    
    IF NOT EXISTS (SELECT 1 FROM categoria  WHERE id_categoria = @id_categoria)
        RETURN NULL;

    IF (@mes NOT BETWEEN 1 AND 12)
        RETURN NULL;

    DECLARE @total decimal(12,2);

    SELECT @total = ISNULL(SUM(t.monto),0)
    FROM subcategoria s
    INNER JOIN presupuesto_detalle pd
        ON pd.id_subcategoria = s.id_subcategoria
    INNER JOIN transaccion t
        ON t.id_presupuesto_detalle = pd.id
    WHERE s.id_categoria = @id_categoria
      AND t.anio_transaccion = @anio
      AND t.mes_transaccion = @mes;
    RETURN @total;
END
GO

-- DIAS HASTA VENCIMIENTO
CREATE OR ALTER FUNCTION fn_dias_hasta_vencimiento (@id_obligacion int)
RETURNS int
AS
BEGIN
    
    IF NOT EXISTS (SELECT 1 FROM obligacion_fija  WHERE id_obligacion = @id_obligacion)
        RETURN NULL;

    DECLARE @dias int;

    SELECT @dias = DATEDIFF(DAY, GETDATE(), fecha_finalizacion)
    FROM obligacion_fija
    WHERE id_obligacion = @id_obligacion;

    RETURN @dias;
    END
GO

-- VALIDAR VIGENCIA
CREATE OR ALTER FUNCTION fn_validar_vigencia_presupuesto(@fecha date, @id_presupuesto int)
RETURNS bit
AS
BEGIN
    DECLARE @periodo_inicio INT,
            @periodo_fin INT,
            @periodo_fecha INT,
            @resultado BIT;

    SELECT
        @periodo_inicio = anio_inicio * 100 + mes_inicio,
        @periodo_fin    = anio_fin * 100 + mes_fin
    FROM presupuesto
    WHERE presupuesto_id = @id_presupuesto;

    SET @periodo_fecha = YEAR(@fecha) * 100 + MONTH(@fecha);

    IF @periodo_fecha BETWEEN @periodo_inicio AND @periodo_fin
        SET @resultado = 1;
    ELSE
        SET @resultado = 0;

    RETURN @resultado;
    END
GO

-- OBTENER CATEGORIA POR SUBCATEGORIA
CREATE OR ALTER FUNCTION fn_obtener_categoria_por_subcategoria (@id_subcategoria int)
RETURNS int
AS
BEGIN

    IF NOT EXISTS (SELECT 1 FROM subcategoria WHERE id_subcategoria = @id_subcategoria)
        RETURN NULL;

    DECLARE @id_categoria int;

    SELECT @id_categoria = id_categoria
    FROM subcategoria
    WHERE id_subcategoria = @id_subcategoria;

    RETURN @id_categoria;
END
GO


-- CALCULAR PROYECCION MENSUAL
CREATE OR ALTER FUNCTION fn_calcular_proyeccion_gasto_mensual(@id_subcategoria int, @anio smallint, @mes tinyint)
RETURNS decimal(12,2)
AS
BEGIN

    IF NOT EXISTS (SELECT 1 FROM subcategoria WHERE id_subcategoria = @id_subcategoria)
        RETURN NULL;

    IF (@mes NOT BETWEEN 1 AND 12)
        RETURN NULL;

    DECLARE @ejecutado decimal(12,2);
    DECLARE @dias_transcurridos int;
    DECLARE @dias_mes int;

    SELECT @ejecutado = dbo.fn_calcular_monto_ejecutado(@id_subcategoria,@anio,@mes);

    SET @dias_transcurridos = DAY(GETDATE());
    SET @dias_mes = DAY(EOMONTH(GETDATE()));

    RETURN CASE
        WHEN @dias_transcurridos = 0
        THEN 0
        ELSE (@ejecutado / @dias_transcurridos) * @dias_mes
    END;
END
GO

-- PROMEDIO GASTO
CREATE OR ALTER FUNCTION fn_obtener_promedio_gasto_subcategoria (@id_usuario int, @id_subcategoria int, @cantidad_meses int)
RETURNS decimal(12,2)
AS
BEGIN
    DECLARE @promedio decimal(12,2);

    IF NOT EXISTS (SELECT 1 FROM subcategoria WHERE id_subcategoria = @id_subcategoria)
        RETURN NULL;
    IF NOT EXISTS (SELECT 1 FROM usuario WHERE usuario_id = @id_usuario)
        RETURN NULL;

    SELECT @promedio = AVG(monto_total)
    FROM
    (
        SELECT SUM(t.monto) monto_total
        FROM transaccion t
        INNER JOIN presupuesto_detalle pd
            ON pd.id = t.id_presupuesto_detalle
        INNER JOIN presupuesto p
            ON p.presupuesto_id = pd.id_presupuesto
        WHERE p.usuario_id = @id_usuario
          AND pd.id_subcategoria = @id_subcategoria
        GROUP BY t.anio_transaccion, t.mes_transaccion
    ) datos;

    RETURN ISNULL(@promedio,0);
END
GO