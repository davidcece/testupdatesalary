DELIMITER $$

CREATE PROCEDURE UpdateEmployeeSalary(
    IN empID INT,
    IN newSalary INT,
    OUT oldSalary INT,
    OUT statusCode INT
)
        main: BEGIN

        DECLARE storedId INT;

        SELECT
            id,
            salary
        INTO storedId, oldSalary
        FROM employees
        WHERE id = empID FOR UPDATE;

        IF storedId IS NULL
        THEN
            /* Employee not found */
            SET statusCode = 1;
            LEAVE main;
        END IF;

        UPDATE employees
        SET salary = newSalary
        WHERE id = empID;
        COMMIT;
        SET statusCode = 0;
    END $$

DELIMITER ;
