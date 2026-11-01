'use strict';

// ---------------------------------------------------------------------------
// Settings
// ---------------------------------------------------------------------------
const DEFAULT_API_BASE = 'http://localhost:5080';
const STORAGE_KEY = 'dealershipClient.apiBase';

// One entry per table / controller. "columns" decides what the Get All table shows.
const RESOURCES = {
  dealerships: {
    singular: 'dealership',
    plural: 'dealerships',
    path: '/api/dealerships',
    title: (r) => r.name,
    columns: [
      { key: 'id', label: 'ID', numeric: true },
      { key: 'name', label: 'Name' },
      { key: 'city', label: 'City' }
    ]
  },
  salespeople: {
    singular: 'salesperson',
    plural: 'salespeople',
    path: '/api/salespeople',
    title: (r) => r.fullName,
    columns: [
      { key: 'id', label: 'ID', numeric: true },
      { key: 'fullName', label: 'Full name' },
      { key: 'email', label: 'Email' },
      { key: 'dealershipName', label: 'Dealership' }
    ]
  },
  vehicles: {
    singular: 'vehicle',
    plural: 'vehicles',
    path: '/api/vehicles',
    title: (r) => r.description,
    columns: [
      { key: 'id', label: 'ID', numeric: true },
      { key: 'description', label: 'Vehicle' },
      { key: 'vin', label: 'VIN' },
      { key: 'priceDisplay', label: 'Price', numeric: true },
      { key: 'dealershipName', label: 'Dealership' }
    ]
  }
};

const FIELD_LABELS = {
  id: 'ID',
  name: 'Name',
  city: 'City',
  dealershipId: 'Dealership ID',
  dealershipName: 'Dealership',
  fullName: 'Full name',
  email: 'Email',
  vin: 'VIN',
  make: 'Make',
  model: 'Model',
  year: 'Year',
  price: 'Price',
  description: 'Description',
  priceDisplay: 'Price (formatted)'
};

const STATUS_TEXT = { 200: 'OK', 201: 'Created', 204: 'No Content', 400: 'Bad Request', 404: 'Not Found', 500: 'Internal Server Error' };

// ---------------------------------------------------------------------------
// Page state: each tab remembers its own last results.
// ---------------------------------------------------------------------------
const state = {
  resource: 'dealerships',
  results: { dealerships: {}, salespeople: {}, vehicles: {} }
};

const el = {
  apiForm: document.getElementById('api-form'),
  apiBase: document.getElementById('api-base'),
  tabs: Array.from(document.querySelectorAll('[role="tab"]')),
  getAll: document.getElementById('get-all'),
  getOne: document.getElementById('get-one'),
  getOneButton: document.getElementById('get-one-button'),
  recordId: document.getElementById('record-id'),
  status: document.getElementById('status'),
  single: document.getElementById('single'),
  list: document.getElementById('list'),
  listTitle: document.getElementById('list-title'),
  listHead: document.getElementById('list-head'),
  listBody: document.getElementById('list-body')
};

// ---------------------------------------------------------------------------
// API address (remembered in this browser if storage is available)
// ---------------------------------------------------------------------------
function readSavedApiBase() {
  try {
    return window.localStorage.getItem(STORAGE_KEY);
  } catch {
    return null;
  }
}

function saveApiBase(value) {
  try {
    window.localStorage.setItem(STORAGE_KEY, value);
  } catch {
    // Storage can be blocked (private windows); the value still works for this visit.
  }
}

function apiBase() {
  return (el.apiBase.value || DEFAULT_API_BASE).trim().replace(/\/+$/, '');
}

// ---------------------------------------------------------------------------
// HTTP: one GET helper used by every button.
// ---------------------------------------------------------------------------
async function getJson(path) {
  const url = apiBase() + path;
  const started = performance.now();

  let response;
  try {
    response = await fetch(url, { headers: { Accept: 'application/json' } });
  } catch {
    // fetch only throws when no HTTP response reached the page:
    // the API isn't running, the address is wrong, or the browser blocked it (CORS).
    return { url, networkError: true, ms: Math.round(performance.now() - started) };
  }

  const ms = Math.round(performance.now() - started);
  const text = await response.text();
  let body = null;
  if (text) {
    try {
      body = JSON.parse(text);
    } catch {
      body = text;
    }
  }

  return { url, ok: response.ok, status: response.status, statusText: response.statusText || STATUS_TEXT[response.status] || '', body, ms };
}

// ---------------------------------------------------------------------------
// Actions
// ---------------------------------------------------------------------------
async function loadAll() {
  const resource = RESOURCES[state.resource];
  const key = state.resource;
  setBusy(true);
  showStatus({ kind: 'pending', text: `GET ${apiBase()}${resource.path} …` });

  const result = await getJson(resource.path);
  const slot = state.results[key];

  if (result.ok && Array.isArray(result.body)) {
    slot.list = result.body;
    slot.status = { kind: 'ok', text: `${describe(result)} · ${result.body.length} ${resource.plural} returned` };
  } else {
    slot.status = errorStatus(result, resource);
  }

  setBusy(false);
  if (key === state.resource) render();
}

async function loadOne(id) {
  const resource = RESOURCES[state.resource];
  const key = state.resource;
  setBusy(true);
  showStatus({ kind: 'pending', text: `GET ${apiBase()}${resource.path}/${id} …` });

  const result = await getJson(`${resource.path}/${id}`);
  const slot = state.results[key];

  if (result.ok && result.body && typeof result.body === 'object') {
    slot.single = result.body;
    slot.status = { kind: 'ok', text: `${describe(result)} · ${resource.singular} ${id} returned` };
  } else {
    slot.single = null;
    slot.status = result.status === 404
      ? { kind: 'error', text: `${describe(result)} · no ${resource.singular} has ID ${id}` }
      : errorStatus(result, resource);
  }

  setBusy(false);
  if (key === state.resource) render();
}

function describe(result) {
  return `GET ${result.url} → ${result.status} ${result.statusText} (${result.ms} ms)`;
}

function errorStatus(result, resource) {
  if (result.networkError) {
    return {
      kind: 'error',
      text: `GET ${result.url} failed: no response reached this page. ` +
        `Check that the API is running at ${apiBase()} and that its Cors:AllowedOrigins setting includes ${window.location.origin}. ` +
        `Tip: open the URL in a new tab. If it shows JSON there, the problem is CORS.`
    };
  }

  const detail = result.body && typeof result.body === 'object'
    ? [result.body.title, result.body.detail].filter(Boolean).join(' ')
    : String(result.body || '');
  return { kind: 'error', text: `${describe(result)}${detail ? ' · ' + detail : ''}` };
}

// ---------------------------------------------------------------------------
// Rendering
// ---------------------------------------------------------------------------
function render() {
  const resource = RESOURCES[state.resource];
  const slot = state.results[state.resource];

  el.tabs.forEach((tab) => tab.setAttribute('aria-selected', String(tab.dataset.resource === state.resource)));
  el.getAll.textContent = `Get all ${resource.plural}`;
  el.getOneButton.textContent = `Get ${resource.singular} by ID`;

  if (slot.status) {
    showStatus(slot.status);
  } else {
    el.status.hidden = true;
  }

  renderSingle(resource, slot.single);
  renderList(resource, slot.list);
}

function renderSingle(resource, record) {
  el.single.replaceChildren();
  if (!record) {
    el.single.hidden = true;
    return;
  }

  const label = document.createElement('p');
  label.className = 'label';
  label.textContent = `Get by ID · ${resource.singular} ${record.id}`;

  const heading = document.createElement('h2');
  heading.textContent = resource.title(record);

  const fields = document.createElement('dl');
  Object.entries(record).forEach(([key, value]) => {
    const term = document.createElement('dt');
    term.textContent = FIELD_LABELS[key] || key;
    const detail = document.createElement('dd');
    detail.textContent = value === null || value === undefined ? '—' : String(value);
    fields.append(term, detail);
  });

  el.single.append(label, heading, fields);
  el.single.hidden = false;
}

function renderList(resource, rows) {
  if (!rows) {
    el.list.hidden = true;
    return;
  }

  el.listTitle.textContent = `Get all · ${rows.length} ${resource.plural}`;

  const headRow = document.createElement('tr');
  resource.columns.forEach((column) => {
    const th = document.createElement('th');
    th.scope = 'col';
    th.textContent = column.label;
    if (column.numeric) th.className = 'num';
    headRow.append(th);
  });
  el.listHead.replaceChildren(headRow);

  const bodyRows = rows.map((row) => {
    const tr = document.createElement('tr');
    tr.title = `Load ${resource.singular} ${row.id}`;
    tr.addEventListener('click', () => {
      el.recordId.value = row.id;
      loadOne(row.id);
    });
    resource.columns.forEach((column) => {
      const td = document.createElement('td');
      const value = row[column.key];
      td.textContent = value === null || value === undefined ? '—' : String(value);
      if (column.numeric) td.className = 'num';
      tr.append(td);
    });
    return tr;
  });
  el.listBody.replaceChildren(...bodyRows);
  el.list.hidden = false;
}

function showStatus(status) {
  el.status.textContent = status.text;
  el.status.className = `status ${status.kind}`;
  el.status.hidden = false;
}

function setBusy(busy) {
  el.getAll.disabled = busy;
  el.getOneButton.disabled = busy;
}

// ---------------------------------------------------------------------------
// Wire up the page
// ---------------------------------------------------------------------------
el.apiBase.value = readSavedApiBase() || DEFAULT_API_BASE;

el.apiForm.addEventListener('submit', (event) => {
  event.preventDefault();
  el.apiBase.value = apiBase();
  saveApiBase(el.apiBase.value);
  showStatus({ kind: 'ok', text: `Requests will go to ${el.apiBase.value}` });
});

el.tabs.forEach((tab) => {
  tab.addEventListener('click', () => {
    state.resource = tab.dataset.resource;
    render();
  });
});

el.getAll.addEventListener('click', loadAll);

el.getOne.addEventListener('submit', (event) => {
  event.preventDefault();
  const id = Number.parseInt(el.recordId.value, 10);
  if (Number.isInteger(id) && id > 0) {
    loadOne(id);
  }
});

render();
