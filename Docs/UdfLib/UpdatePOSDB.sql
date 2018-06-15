CREATE FUNCTION TAGMSG RETURNS INTEGER SONAME 'UDFMessenger.dll';
CREATE FUNCTION SECMSG RETURNS INTEGER SONAME 'UDFMessenger.dll';
CREATE FUNCTION POSMSG RETURNS INTEGER SONAME 'UDFMessenger.dll';
 
delimiter $$
 
CREATE TRIGGER tags_before_update BEFORE UPDATE ON tags
FOR EACH ROW BEGIN
    DECLARE tagId INT(10);
    DECLARE tagStatus INT(10);
    SET tagId=NEW.id;
    SET tagStatus=NEW.status;
    SET @r=TAGMSG(tagId,tagStatus);
END$$
 
CREATE TRIGGER sector_status_before_insert BEFORE INSERT ON sector_status
FOR EACH ROW BEGIN
    DECLARE sectorId INT(10);
    DECLARE sectorStatus INT(10);
    SET sectorId=NEW.sectorid;
    SET sectorStatus=NEW.status;
    SET @r=SECMSG(sectorId,sectorStatus);
END$$
 
CREATE TRIGGER pos_before_insert BEFORE INSERT ON pos
FOR EACH ROW BEGIN
    DECLARE tagId INT(10);
    DECLARE sectorId INT(10);
    SET tagId=NEW.tagid;
    SET sectorId=NEW.sectorid;
    SET @r=POSMSG(tagId,sectorId);
END$$
 
delimiter ;