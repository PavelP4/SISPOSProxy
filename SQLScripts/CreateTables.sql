USE pos;

--
-- Create table "ilasst"
--
CREATE TABLE real_ilasst (
  id int(11) UNSIGNED NOT NULL,
  address varchar(32) NOT NULL,
  port smallint(5) UNSIGNED NOT NULL,
  PRIMARY KEY (id),
  UNIQUE INDEX address_UNIQUE (address),
  UNIQUE INDEX id_UNIQUE (id)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 8192
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;

--
-- Create table "settings"
--
CREATE TABLE proxy_settings (
  name varchar(45) NOT NULL,
  value varchar(45) NOT NULL,
  description varchar(128) DEFAULT NULL,
  PRIMARY KEY (name)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 1260
CHARACTER SET utf8
COLLATE utf8_general_ci
ROW_FORMAT = DYNAMIC;