/**
 * Frontend API Service to interact with Weather-Server backend.
 */

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'https://localhost:5727';

export async function registerUser(email, password) {
  const response = await fetch(`${API_BASE_URL}/api/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  });
  if (response.ok) {
    return true;
  } else {
    const error = await response.text();
    throw new Error(`Registration failed: ${error}`);
  }
}

export async function loginUser(email, password) {
  const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  });
  if (response.ok) {
    return true;
  } else {
    throw new Error('Login failed');
  }
}

export async function logoutUser() {
  const response = await fetch(`${API_BASE_URL}/api/auth/logout`, {
    method: 'POST',
  });
  if (!response.ok) {
    throw new Error('Logout failed');
  }
}

export async function getWeatherByCity(city) {
  const response = await fetch(`${API_BASE_URL}/api/weather/city/${encodeURIComponent(city)}`);
  if (response.ok) {
    const weatherData = await response.json();
    return weatherData;
  } else {
    throw new Error(`Weather data not found for city: ${city}`);
  }
}
