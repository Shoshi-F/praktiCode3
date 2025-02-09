import axios from 'axios';

axios.defaults.baseURL = "http://localhost:5146"

axios.interceptors.response.use(
  response => response,
  error => {
      console.error("API Error:", error.response?.status, error.message);
      return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get(`/ToDos`)    
    return result.data;
  },

  addTask: async(name)=>{
    console.log('addTask', )
    await axios.post(`/ToDos`,{name:name,isComplete:false})
    return {};
  },

  setCompleted: async(id,isComplete)=>{
    const result = await axios.put(`/ToDos/${id}?iscomplete=${isComplete}`, {});
    return result.data;
  },

  deleteTask:async(id)=>{
    await axios.delete(`/ToDos/${id}`,id)
  }
};