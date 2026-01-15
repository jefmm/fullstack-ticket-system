// ==== Konfiguration ====
// Tipp: Wenn du später deployest, kannst du das via ENV/Build ersetzen.
// Für jetzt: eine zentrale Stelle reicht.
const API_BASE_URL = "http://localhost:5088";   // ggf. anpassen
const CREATE_TICKET_PATH = "/tickets";          // ggf. anpassen

const form = document.getElementById("ticketForm");
const banner = document.getElementById("banner");
const submitBtn = document.getElementById("submitBtn");
const resetBtn = document.getElementById("resetBtn");

const nameEl = document.getElementById("name");
const emailEl = document.getElementById("email");
const messageEl = document.getElementById("message");

function showBanner(type, text) {
  banner.className = `banner ${type}`;
  banner.textContent = text;
  banner.hidden = false;
}

function hideBanner() {
  banner.hidden = true;
  banner.textContent = "";
  banner.className = "banner";
}

function validate() {
  hideBanner();

  const name = nameEl.value.trim();
  const email = emailEl.value.trim();
  const message = messageEl.value.trim();

  if (name.length < 2) return { ok: false, msg: "Name ist zu kurz (mind. 2 Zeichen)." };
  // simple email check (HTML type=email ist nicht immer streng)
  if (!/^\S+@\S+\.\S+$/.test(email)) return { ok: false, msg: "Bitte eine gültige E-Mail angeben." };
  if (message.length < 10) return { ok: false, msg: "Nachricht ist zu kurz (mind. 10 Zeichen)." };

  return { ok: true, data: { name, email, message } };
}

function setLoading(isLoading) {
  submitBtn.disabled = isLoading;
  resetBtn.disabled = isLoading;

  submitBtn.textContent = isLoading ? "Sende…" : "Ticket absenden";
}

async function createTicket(payload) {
  const url = `${API_BASE_URL}${CREATE_TICKET_PATH}`;

  const res = await fetch(url, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });

  // Fehlertext lesbar machen:
  if (!res.ok) {
    let details = "";
    try {
      const t = await res.text();
      details = t ? ` (${t})` : "";
    } catch {}
    throw new Error(`Request fehlgeschlagen: ${res.status} ${res.statusText}${details}`);
  }

  // Wenn dein Backend JSON zurückgibt (z.B. Ticket-ID), nutzen wir das:
  const contentType = res.headers.get("content-type") || "";
  if (contentType.includes("application/json")) {
    return await res.json();
  }
  return null;
}

form.addEventListener("submit", async (e) => {
  e.preventDefault();

  const v = validate();
  if (!v.ok) {
    showBanner("bad", v.msg);
    return;
  }

  setLoading(true);
  try {
    const result = await createTicket(v.data);

    // Optional: Ticket-ID anzeigen, falls vorhanden
    const id = result && (result.id || result.ticketId || result.uuid);
    showBanner("ok", id ? `Ticket erstellt ✅ (ID: ${id})` : "Ticket erstellt ✅");
    form.reset();
  } catch (err) {
    showBanner("bad", err?.message || "Unbekannter Fehler");
  } finally {
    setLoading(false);
  }
});

resetBtn.addEventListener("click", () => {
  hideBanner();
  form.reset();
});
