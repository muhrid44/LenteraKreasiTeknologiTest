create table passwordhashtable (
id serial primary key,
name varchar,
passwordhash bytea,
passwordsalt bytea
);

create table departemen (
id serial primary key,
name varchar
);

create table jabatan (
id serial primary key,
name varchar
);

create table karyawan (
nik varchar primary key,
name varchar,
address varchar,
birthdate timestamp,
gender varchar,
departemen_id int,
jabatan_id int,
constraint fk_departemen
foreign key(departemen_id)
references departemen(id),
constraint fk_jabatan
foreign key(jabatan_id)
references jabatan(id)
);

insert into departemen (name) values ('IT'), ('Sales')

insert into jabatan (name) values ('Staff'), ('Manager')

insert into karyawan (nik, name, address, birthdate, gender, departemen_id, jabatan_id)
values ('2020100001','Albert', 'Ciputat', '15-12-1986', 'Laki-Laki', 1,1)

insert into karyawan (nik, name, address, birthdate, gender, departemen_id, jabatan_id)
values ('2020200001','Sylpha', 'Surabaya', '20-06-1918', 'Perempuan', 2,2)

