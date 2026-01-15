const form = document.getElementById("ticketForm");
const result = document.getElementById("result");

form.addEventListener("submit", async (event) => {
  event.preventDefault();

  const ticket = {
    name: document.getElementById("name").value,
    email: document.getElementById("email").value,
    message: document.getElementById("message").value
  };

  try {
    const response = await fetch("http://localhost:5088/tickets", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(ticket)
    });

    if (response.ok) {
      result.textContent = "✅ Ticket erfolgreich erstellt!";
      form.reset();
    } else {
      result.textContent = "❌ Fehler beim Erstellen des Tickets.";
    }

  } catch (error) {
  console.error("FETCH ERROR:", error);
  result.textContent = "❌ Server nicht erreichbar: " + error;
}

});
