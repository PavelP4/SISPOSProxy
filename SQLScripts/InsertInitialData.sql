USE pos;

START TRANSACTION;

-- real ilasst settings

INSERT INTO real_ilasst (id, address, port)
VALUES (1, '192.168.10.133', 55554);

INSERT INTO real_ilasst (id, address, port)
VALUES (2, '192.168.10.134', 55555);

-- named pipe settings

INSERT INTO proxy_settings (name, value, description)
VALUES ('udf2proxy_namedpipe_name', 'SIS_POS_UDF_Messenger', 'The pipe name that is used by a udf to communicate with the proxy');

INSERT INTO proxy_settings (name, value, description)
VALUES ('udf2proxy_namedpipe_maxserverinstances', '4', 'Max named pipe server instances');


COMMIT;