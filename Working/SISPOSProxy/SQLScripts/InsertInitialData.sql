USE pos;

START TRANSACTION;

-- real ilasst settings

INSERT INTO real_ilasst (id, address, port)
SELECT id, address, port FROM ilasst;

-- ilasst settings

UPDATE ilasst
 SET address = concat('127.0.0.', id),
	 port = 50000 + id;

-- named pipe settings

INSERT INTO proxy_settings (name, value, description)
VALUES ('udf2proxy_namedpipe_name', 'SIS_POS_UDF_Messenger', 'The pipe name that is used by a udf to communicate with the proxy');

INSERT INTO proxy_settings (name, value, description)
VALUES ('udf2proxy_namedpipe_maxserverinstances', '4', 'Max named pipe server instances');


COMMIT;