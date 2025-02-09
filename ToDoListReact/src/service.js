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
    await axios.post(`/ToDos`,{Name:name,isComplete:false})
    return {};
  },

  setCompleted: async (id, isComplete,name) => {
    await axios.put(`/Todos/${id}`, {name:name,isComplete: isComplete });
  },


  deleteTask:async(id)=>{
    await axios.delete(`/ToDos/${id}`,id)
  }
};