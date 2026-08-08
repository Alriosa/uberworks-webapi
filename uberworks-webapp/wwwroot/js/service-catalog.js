// =====================================================================================
// FILE SUMMARY
// What it does: Shared data for the 24 service categories (Carpintería, Fontanero, ...,
//               Asistente de Ventas). Used by LandingPage.cshtml (as a fallback when a
//               search doesn't match one of the 6 curated cards) and by AllServices.cshtml
//               (the full category grid) — keeping it in one file means both pages show the
//               exact same title/description/photo for a given category instead of two
//               copies drifting apart over time. Each photo was sourced from Unsplash and
//               reviewed one by one for relevance to its category.
// Entities connected: None
// Tables related: None
// =====================================================================================
const serviceCatalog = [
    { key: 'Carpintería', desc: 'Reparación y armado de muebles, estantes y estructuras de madera.', icon: 'fa-hammer', img: 'https://images.unsplash.com/photo-1631396326646-c06a935ff3a6?auto=format&fit=crop&w=800&q=80' },
    { key: 'Fontanero', desc: 'Reparación de fugas, tuberías y grifería a domicilio.', icon: 'fa-faucet-drip', img: 'https://images.unsplash.com/photo-1607472586893-edb57bdc0e39?auto=format&fit=crop&w=800&q=80' },
    { key: 'Electricista', desc: 'Instalaciones eléctricas, cortocircuitos y mantenimiento de tableros.', icon: 'fa-bolt', img: 'https://images.unsplash.com/photo-1635335874521-7987db781153?auto=format&fit=crop&w=800&q=80' },
    { key: 'Tapicería', desc: 'Restauración y forrado de muebles y sillas.', icon: 'fa-couch', img: 'https://images.unsplash.com/photo-1616627547584-bf28cee262db?auto=format&fit=crop&w=800&q=80' },
    { key: 'Reajustes y Remodelaciones', desc: 'Ajustes menores y remodelaciones generales del hogar.', icon: 'fa-screwdriver-wrench', img: 'https://images.unsplash.com/photo-1505798577917-a65157d3320a?auto=format&fit=crop&w=800&q=80' },
    { key: 'Fumigación', desc: 'Control de plagas e insectos en tu hogar o negocio.', icon: 'fa-bug', img: 'https://images.unsplash.com/photo-1629608934925-725d09a4eb9a?auto=format&fit=crop&w=800&q=80' },
    { key: 'Limpieza', desc: 'Limpieza general y profunda del hogar.', icon: 'fa-broom', img: 'https://images.unsplash.com/photo-1581578949510-fa7315c4c350?auto=format&fit=crop&w=800&q=80' },
    { key: 'Lava Platos', desc: 'Personal de apoyo para lavado de platos en eventos o cocinas.', icon: 'fa-sink', img: 'https://images.unsplash.com/photo-1590610994353-7b0e7546e681?auto=format&fit=crop&w=800&q=80' },
    { key: 'Jardinería', desc: 'Mantenimiento de jardines, poda y paisajismo.', icon: 'fa-seedling', img: 'https://images.unsplash.com/photo-1621272156568-7306716648df?auto=format&fit=crop&w=800&q=80' },
    { key: 'Pasea Perros', desc: 'Paseos programados para tu mascota.', icon: 'fa-dog', img: 'https://images.unsplash.com/photo-1529472119196-cb724127a98e?auto=format&fit=crop&w=800&q=80' },
    { key: 'Hotel de Perros', desc: 'Alojamiento temporal y cuidado para tu perro.', icon: 'fa-house', img: 'https://images.unsplash.com/photo-1743763959056-41bbb557272d?auto=format&fit=crop&w=800&q=80' },
    { key: 'Pet Grooming', desc: 'Baño, corte y cuidado estético para mascotas.', icon: 'fa-scissors', img: 'https://images.unsplash.com/photo-1561037404-61cd46aa615b?auto=format&fit=crop&w=800&q=80' },
    { key: 'Cocineros', desc: 'Chefs a domicilio para eventos o el día a día.', icon: 'fa-utensils', img: 'https://images.unsplash.com/photo-1577219492769-b63a779fac28?auto=format&fit=crop&w=800&q=80' },
    { key: 'Ayudante de Cocina', desc: 'Apoyo en preparación y organización de cocina.', icon: 'fa-kitchen-set', img: 'https://images.unsplash.com/photo-1554997433-8e233c02c751?auto=format&fit=crop&w=800&q=80' },
    { key: 'Bartenders', desc: 'Preparación de bebidas y cocteles para tu evento.', icon: 'fa-martini-glass', img: 'https://images.unsplash.com/photo-1500217052183-bc01eee1a74e?auto=format&fit=crop&w=800&q=80' },
    { key: 'Mesero', desc: 'Personal de servicio de mesa para eventos.', icon: 'fa-bell-concierge', img: 'https://images.unsplash.com/photo-1710082936223-9f60e6842a3e?auto=format&fit=crop&w=800&q=80' },
    { key: 'Ayudante de Caja', desc: 'Apoyo en cobro y manejo de caja para tu negocio o evento.', icon: 'fa-cash-register', img: 'https://images.unsplash.com/photo-1647427017458-f6df91d046eb?auto=format&fit=crop&w=800&q=80' },
    { key: 'Staff para Eventos', desc: 'Personal de apoyo general para la logística de tu evento.', icon: 'fa-people-group', img: 'https://images.unsplash.com/photo-1641122669951-3e2aff778d3b?auto=format&fit=crop&w=800&q=80' },
    { key: 'Coordinador del Evento', desc: 'Planificación y coordinación integral de tu evento.', icon: 'fa-clipboard-list', img: 'https://images.unsplash.com/photo-1541140911322-98afe66ea6da?auto=format&fit=crop&w=800&q=80' },
    { key: 'Promotoras', desc: 'Personal de impulso y promoción de marca para tu evento.', icon: 'fa-bullhorn', img: 'https://images.unsplash.com/photo-1771979788428-6590f3da8491?auto=format&fit=crop&w=800&q=80' },
    { key: 'DJ', desc: 'Música y ambientación profesional para tu evento.', icon: 'fa-headphones-simple', img: 'https://images.unsplash.com/photo-1470225620780-dba8ba36b745?auto=format&fit=crop&w=800&q=80' },
    { key: 'Animadores Infantiles', desc: 'Animación y entretenimiento para fiestas infantiles.', icon: 'fa-child', img: 'https://images.unsplash.com/photo-1519340241574-2cec6aef0c01?auto=format&fit=crop&w=800&q=80' },
    { key: 'Mudanzas', desc: 'Transporte y traslado de muebles y pertenencias.', icon: 'fa-boxes-stacked', img: 'https://images.unsplash.com/photo-1600725935160-f67ee4f6084a?auto=format&fit=crop&w=800&q=80' },
    { key: 'Asistente de Ventas', desc: 'Apoyo en ventas y atención al cliente para tu negocio.', icon: 'fa-tags', img: 'https://images.unsplash.com/photo-1556740738-b6a63e27c4df?auto=format&fit=crop&w=800&q=80' }
];
