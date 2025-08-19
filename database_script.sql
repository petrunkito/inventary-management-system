



create table categories
(
	id uuid default gen_random_uuid() primary key,
	title varchar(25) not null unique,
	description varchar(80),
	is_active boolean default true not null,
	created_at timestamp default LOCALTIMESTAMP not null
);

create table suppliers
(
	id uuid default gen_random_uuid() primary key,
	name varchar(50) not null unique,
	number_phone varchar(9),
	address varchar(150),
	is_active boolean default true not null,
	created_at timestamp default LOCALTIMESTAMP not null
);

create table products
(
	id uuid default gen_random_uuid() primary key,
	category_id uuid not null,
	supplier_id uuid not null, 
	name varchar(50) not null,
	description varchar(200),
	sale_price numeric(10, 2) not null, 
	stock int default 0 not null,
	is_active boolean default true not null,
	created_at timestamp default LOCALTIMESTAMP not null,
	constraint fK_category_id
		foreign key (category_id)
		references categories(id),
	constraint fk_supplier_id
		foreign key (supplier_id)
		references suppliers(id)
		
);

create table type_inputs
(
	id uuid default gen_random_uuid() primary key,
	title varchar(15) not null,
	code smallint not null unique, 
	description varchar(50),
	is_active boolean default true not null,
	created_at timestamp default LOCALTIMESTAMP not null
);
insert into type_inputs (title, code, description) values
('Compra', 0, 'Se está comprando el producto a un proveedor'),
('Devolución', 1, 'Se devuelve el producto al almacén');

create table type_outputs
(
	id uuid default gen_random_uuid() primary key,
	title varchar(15) not null,
	code smallint not null unique, 
	description varchar(50),
	is_active boolean default true not null,
	created_at timestamp deafult LOCALTIMESTAMP not null
);

insert into type_outputs (title, code, description) values
('Venta', 0, 'Se esta realizando una venta'),
('Perdida', 1, 'Productos dañados, robados, pedidos, etc');

create table product_inputs
(
	id uuid default gen_random_uuid() primary key,
	product_id uuid not null,
	input_type_id uuid not null,
	amount int not null,
	cost_price numeric(10,2) not null,
	is_active boolean default true not null,
	created_at timestamp default LOCALTIMESTAMP not null, 	
	constraint fk_product_id
		foreign key (product_id)
		references products(id),
	constraint fk_input_type_id
		foreign key (input_type_id)
		references type_inputs(id)
	
);

create table customers
(
	id uuid default gen_random_uuid() primary key, 
	name varchar(50) not null, 
	number_phone varchar(9),
	address varchar(200),
	is_active boolean default true not null,
	created_at timestamp default LOCALTIMESTAMP not null
);

create table product_outputs
(
	id uuid default gen_random_uuid() primary key,
	output_type_id uuid not null,
	customer_id uuid not null,
	amount int not null,
	total_price numeric(10, 2) not null,
	date timestamp default LOCALTIMESTAMP not null,
	constraint fk_output_type_id
		foreign key (output_type_id)
		references type_outputs(id),
	constraint fk_customer_id
		foreign key (customer_id)
		references customers(id)
	
);

create table departure_details
(
	id uuid default gen_random_uuid() primary key,
	product_id uuid not null,
	product_output_id uuid not null,
	amount int not null, 
	sale_price numeric(10,2) not null,
	total numeric(10, 2) not null,
	constraint fk_product_id
		foreign key (product_id)
		references products(id),
	constraint fk_product_output_id
		foreign key (product_output_id)
		references product_outputs(id)
	
);
