import axios from 'axios';

// אם יש לך קובץ .env, ודא ש-REACT_APP_API_URL מוגדר שם.
const apiUrl = process.env.REACT_APP_API_URL || 'https://todolistserver-vgxn.onrender.com/';

axios.defaults.baseURL = "https://"+apiUrl;  // הגדרת baseURL לפי משתנה הסביבה או URL ברירת המחדל
console.log("API Base URL:", apiUrl);

// הגדרת axios defaults
axios.defaults.headers.common['Content-Type'] = 'application/json';

// הוספת Interceptor לתפיסת שגיאות ב-response
axios.interceptors.response.use(
  response => response,  // אם הכל בסדר, מחזירים את התגובה כרגיל
  error => {
    // במקרה של שגיאה, לוג את השגיאה
    console.error('API Error:', error.response ? error.response.data : error.message);
    return Promise.reject(error);  // מוחזר Promise עם השגיאה
  }
);

export default {
  getTasks: async () => {
    try {
      const result = await axios.get('/tasks');  // נשתמש ב-baseURL
      console.log(result.data);
      return result.data;
    } catch (error) {
      console.error('Error getting tasks:', error);
      return { error: 'Failed to get tasks' };  // תקן את השגיאה
    }
  },

  addTask: async (name) => {
    console.log('addTask', name);
    try {
      const response = await axios.post('/tasks', { name });
      return response.data;
    } catch (error) {
      console.error('Error adding task:', error);
      return { error: 'Failed to add task' };
    }
  },

  setCompleted: async (id, isComplete, name) => {
    console.log('setCompleted', { id, isComplete, name });
    try {
      const response = await axios.put(`/tasks/${id}`, { 
        isComplete,
        name 
      });
      return response.data;
    } catch (error) {
      console.error('Error setting task completion:', error);
      return { error: 'Failed to update task' };
    }
  },

  deleteTask: async (id) => {
    console.log('deleteTask', id);
    try {
      const response = await axios.delete(`/tasks/${id}`);
      return response.data;
    } catch (error) {
      console.error('Error deleting task:', error);
      return { error: 'Failed to delete task' };
    }
  }
};
