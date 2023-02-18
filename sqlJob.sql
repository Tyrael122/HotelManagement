delimiter |
CREATE EVENT roomStatusUpdater
    ON SCHEDULE EVERY 1 MINUTE
    DO
		BEGIN
			UPDATE room INNER JOIN statustimelinelog ON room.Id = statustimelinelog.RoomId SET room.Status = statustimelinelog.Status
			WHERE statustimelinelog.Date <= now();
		END |
delimiter ;